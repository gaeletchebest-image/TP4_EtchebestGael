using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltimaGuardia.Tests
{
    public class CombatRulesTests
    {
        private readonly List<Object> owned = new List<Object>();
        private EncounterData data;
        [SetUp] public void SetUp()
        {
            data = Clone(Resources.Load<EncounterData>("Encounters/MainEncounter"));
            data.participants = data.participants.Select(s => new EncounterSlot { position = s.position, rotation = s.rotation, combatant = Clone(s.combatant) }).ToArray();
            foreach (var slot in data.participants) slot.combatant.actions = slot.combatant.actions.Select(Clone).ToArray();
            data.inventory = data.inventory.Select(e => new InitialItem { item = Clone(e.item), quantity = e.quantity }).ToArray();
        }
        private T Clone<T>(T source) where T : Object { Assert.IsNotNull(source, "Integrated H02 encounter must exist."); var result = Object.Instantiate(source); owned.Add(result); return result; }
        [TearDown] public void TearDown() { foreach (var asset in owned) Object.DestroyImmediate(asset); owned.Clear(); }
        private CombatSession Session() => new CombatSession(data, 1);
        private void EqualSpeeds(float speed = 20) { foreach (var slot in data.participants) slot.combatant.speed = speed; }
        private static void Complete(CombatSession s, CombatAction action = CombatAction.Basic, CombatantState target = null)
        { Assert.IsTrue(s.TryCommit(action, target ?? s.TargetsFor(action)[0], out string error), error); Assert.IsTrue(s.ApplyImpact()); Assert.IsTrue(s.FinishAction()); }
        [TestCase(28, 1, 6, 22)] [TestCase(28, 1.8, 6, 44)] [TestCase(24, 1, 6, 18)] [TestCase(3, 1.5, 0, 5)] [TestCase(1, 1, 99, 1)]
        public void DamageUsesGddRounding(int attack, double potency, double defense, int expected) => Assert.AreEqual(expected, CombatSession.CalculateDamage(attack, potency, defense));

        [Test] public void InitialStateHasNoActorAndCannotSpendBeforeReady()
        {
            var s = Session(); Assert.IsNull(s.Actor); Assert.IsTrue(s.ClockRunning);
            Assert.IsFalse(s.TryCommit(CombatAction.Basic, s.Fighters[2], out _));
            foreach (var f in s.Fighters) { Assert.AreEqual(0, f.Atb); Assert.AreEqual(0, f.RecoveryRemaining); Assert.AreEqual(f.Definition.MaxHealth, f.Health); Assert.AreEqual(f.Definition.MaxMana, f.Mana); }
            CollectionAssert.AreEqual(new[] { 3, 2, 1, 1, 2 }, s.Inventory.Values);
        }
        [TestCase(20, 5)] [TestCase(28, 3.5714285714285716)]
        public void FillingUsesSpeedPerActiveSecond(float speed, double seconds)
        {
            EqualSpeeds(speed); var s = Session();
            s.AdvanceTime(seconds - 0.001); Assert.IsNull(s.Actor);
            Assert.That(s.Fighters[1].Atb, Is.EqualTo(100 - speed * 0.001).Within(1e-7));
            s.AdvanceTime(0.001); Assert.AreEqual("mage", s.Actor.Definition.Id); Assert.AreEqual(100, s.Actor.Atb);
        }
        [Test] public void CrossingsAreOrderedWithinLargeUpdateAndQueueDrainsBeforeClock()
        {
            var s = Session(); s.AdvanceTime(7); Assert.AreEqual(3, s.ReadyCount);
            string[] expected = { "mage", "warrior", "shaman", "ogre" };
            foreach (string id in expected)
            {
                Assert.AreEqual(id, s.Actor.Definition.Id); double clock = s.ActiveTime;
                s.AdvanceTime(99); Assert.AreEqual(clock, s.ActiveTime);
                Complete(s);
            }
            Assert.AreEqual(0, s.ReadyCount); Assert.IsNull(s.Actor); Assert.IsTrue(s.ClockRunning);
            s.AdvanceTime(0.1); Assert.AreEqual(7.1, s.ActiveTime, 1e-8);
        }
        [Test] public void ExactTiesUseFixedOrderAndNoDuplicateEntries()
        {
            EqualSpeeds(); var s = Session(); s.AdvanceTime(5);
            foreach (string id in new[] { "mage", "warrior", "shaman", "ogre" })
            { Assert.AreEqual(id, s.Actor.Definition.Id); s.AdvanceTime(5); Complete(s); }
            Assert.AreEqual(4, s.CompletedActions); Assert.AreEqual(0, s.ReadyCount);
        }
        [Test] public void SimultaneousCrossingUsesEffectiveSpeedBeforeFixedOrder()
        {
            EqualSpeeds(20); data.participants[0].combatant.speed = 40;
            data.participants[0].combatant.actions[0].recovery = 0;
            var s = Session(); s.AdvanceTime(2.5); Assert.AreEqual("warrior", s.Actor.Definition.Id);
            // Equal crossing in 2.5s: faster warrior precedes mage, despite fixed tie rank.
            Complete(s); s.AdvanceTime(2.5);
            Assert.AreEqual("warrior", s.Actor.Definition.Id); Complete(s);
            Assert.AreEqual("mage", s.Actor.Definition.Id); Complete(s);
            Assert.AreEqual("shaman", s.Actor.Definition.Id); Complete(s);
            Assert.AreEqual("ogre", s.Actor.Definition.Id); Complete(s);
            Assert.IsNull(s.Actor);
        }
        [Test] public void RecoveryConsumesTimeFirstAndUsesRemainderForAtb()
        {
            EqualSpeeds(); var s = Session(); s.AdvanceTime(5);
            Complete(s); var mage = s.Fighters[1];
            Assert.AreEqual(0, mage.Atb); Assert.AreEqual(0.6, mage.RecoveryRemaining, 1e-6);
            Complete(s, CombatAction.HeavyStrike); var warrior = s.Fighters[0];
            Assert.AreEqual(2.2, warrior.RecoveryRemaining, 1e-6);
            Complete(s); Complete(s);
            s.AdvanceTime(0.5); Assert.AreEqual(0, mage.Atb); Assert.AreEqual(0.1, mage.RecoveryRemaining, 1e-6);
            s.AdvanceTime(0.4); Assert.AreEqual(6, mage.Atb, 1e-5); Assert.AreEqual(0, mage.RecoveryRemaining);
            Assert.AreEqual(0, warrior.Atb); Assert.AreEqual(1.3, warrior.RecoveryRemaining, 1e-6);
        }
        [Test] public void DecisionExecutionPauseAndResultFreezeAllTimers()
        {
            var s = Session(); s.IsPaused = true; s.AdvanceTime(99); Assert.AreEqual(0, s.ActiveTime);
            s.IsPaused = false; s.AdvanceTime(4); var bars = s.Fighters.Select(f => f.Atb).ToArray();
            s.AdvanceTime(99); CollectionAssert.AreEqual(bars, s.Fighters.Select(f => f.Atb));
            Assert.IsTrue(s.TryCommit(CombatAction.Basic, s.Fighters[2], out _));
            s.AdvanceTime(99); Assert.AreEqual(4, s.ActiveTime);
            s.IsPaused = true; Assert.IsFalse(s.ApplyImpact()); Assert.IsFalse(s.FinishAction());
            s.IsPaused = false; s.ApplyImpact(); s.FinishAction();
            s.Fighters[2].Damage(999); s.Fighters[3].Damage(999);
            s.AdvanceTime(99); Assert.AreEqual(4, s.ActiveTime); Assert.AreEqual(CombatOutcome.Victory, s.Outcome);
        }
        [Test] public void InvalidTargetsAndRepeatedConfirmImpactAndCloseDoNotDoubleSpend()
        {
            EqualSpeeds(); var s = Session(); s.AdvanceTime(5); Complete(s); // warrior
            var ogre = s.Fighters[2];
            Assert.IsFalse(s.TryCommit(CombatAction.HeavyStrike, s.Fighters[1], out _));
            Assert.IsFalse(s.TryCommit(CombatAction.Basic, Session().Fighters[2], out _));
            Assert.IsFalse(s.TryCommit(CombatAction.Potion, s.Actor, out _));
            Assert.AreEqual(30, s.Actor.Mana); Assert.AreEqual(3, s.Potions);
            Assert.IsTrue(s.TryCommit(CombatAction.HeavyStrike, ogre, out _)); Assert.AreEqual(22, s.Actor.Mana);
            Assert.IsFalse(s.TryCommit(CombatAction.HeavyStrike, ogre, out _)); Assert.IsFalse(s.FinishAction());
            int hp = ogre.Health; Assert.IsTrue(s.ApplyImpact()); Assert.AreEqual(hp - 44, ogre.Health);
            Assert.IsFalse(s.ApplyImpact()); Assert.AreEqual(hp - 44, ogre.Health);
            Assert.IsTrue(s.FinishAction()); Assert.IsFalse(s.FinishAction()); Assert.AreEqual("shaman", s.Actor.Definition.Id);
        }
        [Test] public void PotionIsSharedCappedAndConsumesOnlyOnce()
        {
            EqualSpeeds(); var s = Session(); s.AdvanceTime(5); var warrior = s.Fighters[0]; warrior.Damage(10);
            Assert.IsTrue(s.TryCommit(CombatAction.Potion, warrior, out _)); Assert.AreEqual(2, s.Potions);
            s.ApplyImpact(); Assert.AreEqual(10, s.Pending.EffectiveAmount); Assert.AreEqual(160, warrior.Health); s.FinishAction();
            warrior.Damage(60); Complete(s, CombatAction.Potion, warrior); Assert.AreEqual(1, s.Potions);
            Complete(s); Complete(s); s.AdvanceTime(10);
            while (s.Actor.Definition.Team == CombatTeam.Enemies) Complete(s);
            warrior.Damage(1);
            Complete(s, CombatAction.Potion, warrior); Assert.AreEqual(0, s.Potions);
            while (s.Actor != null && s.Actor.Definition.Team == CombatTeam.Enemies) Complete(s);
            Assert.AreEqual("Sin unidades.", s.UnavailableReason(CombatAction.Potion));
        }
        [Test] public void ResourcesClampAndDeathRemovesQueuedTurnsAndTargets()
        {
            EqualSpeeds(); var s = Session(); var warrior = s.Fighters[0];
            Assert.AreEqual(0, warrior.Heal(50)); Assert.AreEqual(0, warrior.Damage(-1));
            Assert.AreEqual(10, warrior.Damage(10)); Assert.AreEqual(10, warrior.Heal(50)); Assert.AreEqual(0, warrior.RestoreMana(999));
            s.AdvanceTime(5); Assert.AreEqual(3, s.ReadyCount);
            warrior.Damage(999); Assert.AreEqual(2, s.ReadyCount); Assert.AreEqual(0, warrior.Atb);
            Assert.AreEqual(0, warrior.Heal(999)); Assert.AreEqual(0, warrior.RestoreMana(999));
            Complete(s); Assert.AreEqual("shaman", s.Actor.Definition.Id);
            s.Fighters[2].Damage(999); Assert.AreEqual(0, s.ReadyCount);
            Assert.IsTrue(s.CommitEnemyAction()); Assert.AreSame(s.Fighters[1], s.Pending.Target);
            s.ApplyImpact(); s.FinishAction(); Assert.IsNull(s.Actor);
        }
        [Test] public void FinalDeathStopsBeforeNextCommandAndAllowsPendingPresentationToClose()
        {
            var s = Session(); s.AdvanceTime(4); s.Fighters[2].Damage(999); s.Fighters[3].Damage(129);
            Assert.IsTrue(s.TryCommit(CombatAction.Basic, s.Fighters[3], out _)); s.ApplyImpact();
            Assert.AreEqual(CombatOutcome.Victory, s.Outcome); Assert.IsFalse(s.TryCommit(CombatAction.Potion, s.Actor, out _));
            Assert.IsTrue(s.FinishAction()); Assert.IsNull(s.Pending); Assert.IsNull(s.Actor); Assert.IsFalse(s.CommitEnemyAction());
        }
        [Test] public void InsufficientManaLeavesActorAndBasicAvailable()
        {
            data.participants[0].combatant.maxMana = 6; EqualSpeeds(); var s = Session(); s.AdvanceTime(5); Complete(s);
            Assert.IsFalse(s.TryCommit(CombatAction.HeavyStrike, s.Fighters[2], out string reason)); Assert.AreEqual("Maná insuficiente.", reason);
            Assert.AreEqual("warrior", s.Actor.Definition.Id); Assert.AreEqual(1, s.CompletedActions); Assert.IsNull(s.UnavailableReason(CombatAction.Basic));
        }
        [Test] public void InspectorConfigurationChangesRuntimeAndRestartLeavesAssetsUntouched()
        {
            data.participants[0].combatant.maxHealth = 200; data.participants[1].combatant.speed = 40;
            data.participants[0].combatant.actions[1].manaCost = 5;
            data.participants[0].combatant.actions[1].potency = 2;
            data.participants[2].combatant.defense = 10;
            data.inventory[0].quantity = 4; data.inventory[0].item.amount = 17;
            var s = Session(); Assert.AreEqual(200, s.Fighters[0].Health); Assert.AreEqual(4, s.Potions);
            s.AdvanceTime(2.5); Assert.AreEqual("mage", s.Actor.Definition.Id);
            s.Fighters[0].Damage(30); Complete(s, CombatAction.Potion, s.Fighters[0]); Assert.AreEqual(187, s.Fighters[0].Health);
            s.AdvanceTime(2.1); Assert.AreEqual("warrior", s.Actor.Definition.Id);
            Assert.IsTrue(s.TryCommit(CombatAction.HeavyStrike, s.Fighters[2], out _)); Assert.AreEqual(25, s.Actor.Mana);
            s.ApplyImpact(); Assert.AreEqual(46, s.Pending.EffectiveAmount);
            var fresh = Session(); Assert.AreEqual(200, fresh.Fighters[0].Health); Assert.AreEqual(30, fresh.Fighters[0].Mana); Assert.AreEqual(4, fresh.Potions);
            Assert.AreEqual(4, data.inventory[0].quantity); Assert.AreEqual(200, data.participants[0].combatant.maxHealth);
            Assert.AreEqual(0, fresh.CompletedActions); Assert.AreEqual(0, fresh.ReadyCount); Assert.IsNull(fresh.Actor);
        }
        [Test] public void InvalidDataReportsMissingReferencesDuplicatesCostsAndSpeed()
        {
            data.participants[0].combatant.speed = float.NaN;
            data.participants[1].combatant.actions[0].manaCost = -1;
            data.participants[2].combatant.actions[0].id = data.participants[3].combatant.actions[0].id;
            data.participants[3].combatant.prefab = null;
            var errors = data.ValidateData();
            Assert.IsTrue(errors.Any(e => e.Contains("velocidad"))); Assert.IsTrue(errors.Any(e => e.Contains("costo")));
            Assert.IsTrue(errors.Any(e => e.Contains("duplicado"))); Assert.IsTrue(errors.Any(e => e.Contains("prefab")));
            Assert.Throws<ArgumentException>(() => Session());
        }
    }
}
