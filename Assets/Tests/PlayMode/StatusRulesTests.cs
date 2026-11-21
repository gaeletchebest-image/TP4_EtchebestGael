using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltimaGuardia.Tests
{
    public sealed class StatusRulesTests
    {
        private EncounterData data;
        private readonly List<Object> owned = new List<Object>();
        private readonly Dictionary<EffectData, EffectData> effectCopies = new Dictionary<EffectData, EffectData>();
        private T Copy<T>(T source) where T : Object { var result = Object.Instantiate(source); owned.Add(result); return result; }
        private EffectData CopyEffect(EffectData source)
        {
            if (!effectCopies.TryGetValue(source, out var result)) effectCopies[source] = result = Copy(source);
            return result;
        }
        [SetUp] public void SetUp()
        {
            data = Copy(Resources.Load<EncounterData>("Encounters/MainEncounter"));
            data.participants = data.participants.Select(p => new EncounterSlot { combatant = Copy(p.combatant), position = p.position, rotation = p.rotation }).ToArray();
            foreach (var slot in data.participants)
            {
                slot.combatant.actions = slot.combatant.actions.Select(Copy).ToArray();
                foreach (var a in slot.combatant.actions) if (a.appliedEffect != null) a.appliedEffect = CopyEffect(a.appliedEffect);
            }
            data.inventory = data.inventory.Select(e => new InitialItem { item = Copy(e.item), quantity = e.quantity }).ToArray();
            foreach (var e in data.inventory) if (e.item.appliedEffect != null) e.item.appliedEffect = CopyEffect(e.item.appliedEffect);
        }
        [TearDown] public void TearDown() { foreach (var o in owned) Object.DestroyImmediate(o); owned.Clear(); effectCopies.Clear(); }
        private EffectDefinition Effect(StatusKind kind) => new EffectDefinition(data.inventory.Select(e => e.item.appliedEffect).First(e => e != null && e.kind == kind));
        private static StatusState Status(CombatantState f, StatusKind kind) => f.Effects.Single(e => e.Definition.Kind == kind);
        private CombatSession Solo(string id)
        {
            foreach (var p in data.participants) p.combatant.speed = p.combatant.id == id ? 100 : 0.01f;
            return new CombatSession(data, 42);
        }
        private static void Complete(CombatSession s, CombatAction command = CombatAction.Basic, CombatantState target = null)
        {
            Assert.IsTrue(s.TryCommit(command, target ?? s.TargetsFor(command)[0], out var reason), reason);
            Assert.IsTrue(s.ApplyImpact()); Assert.IsTrue(s.FinishAction());
        }
        private static void Next(CombatSession s) { s.AdvanceTime(4); Assert.IsNotNull(s.Actor); }

        [Test] public void PoisonDealsExactlyThreeTurnStartTicksIgnoringCorrosionAndDefense()
        {
            var s = Solo("mage"); var mage = s.Fighters[1];
            mage.ApplyStatus(Effect(StatusKind.Poison)); mage.ApplyStatus(Effect(StatusKind.Corrosion));
            int hp = mage.Health;
            for (int turn = 0; turn < 4; turn++)
            {
                Next(s); Assert.AreSame(mage, s.Actor);
                Assert.AreEqual(hp - 8 * Math.Min(turn + 1, 3), mage.Health);
                Assert.AreEqual(turn < 2, mage.HasStatus(StatusKind.Poison));
                if (turn < 2) Assert.AreEqual(2 - turn, Status(mage, StatusKind.Poison).Remaining);
                Complete(s);
            }
            Assert.AreEqual(0, mage.Effects.Count);
        }
        [TestCase(false)] [TestCase(true)]
        public void LethalPoisonEndsCombatBeforeAllyMenuOrEnemyAi(bool enemy)
        {
            var s = Solo(enemy ? "shaman" : "mage");
            var victim = s.Fighters[enemy ? 3 : 1]; var other = s.Fighters[enemy ? 2 : 0];
            other.Damage(999); victim.Damage(victim.Health - 8);
            victim.ApplyStatus(Effect(StatusKind.Poison)); victim.ApplyStatus(Effect(StatusKind.Haste));
            int mana = victim.Mana; NextOutcome(s);
            Assert.AreEqual(enemy ? CombatOutcome.Victory : CombatOutcome.Defeat, s.Outcome);
            Assert.IsNull(s.Actor); Assert.IsNull(s.Pending); Assert.AreEqual(0, s.ReadyCount);
            Assert.IsFalse(s.CommitEnemyAction()); Assert.IsFalse(s.TryCommit(CombatAction.Basic, other, out _));
            Assert.AreEqual(mana, victim.Mana); Assert.AreEqual(0, s.CompletedActions);
            Assert.AreEqual(0, victim.Effects.Count); Assert.AreEqual(0, victim.Atb); Assert.AreEqual(0, victim.RecoveryRemaining);
            StringAssert.Contains("Derrotado", s.LastEvent);
        }
        private static void NextOutcome(CombatSession s) => s.AdvanceTime(5);
        [Test] public void ConsecutivePoisonDeathsDrainReadyQueueWithoutRecursionOrDeadActor()
        {
            foreach (var p in data.participants) p.combatant.speed = 20;
            var s = new CombatSession(data);
            foreach (var id in new[] { 1, 0, 3 })
            { var f = s.Fighters[id]; f.Damage(f.Health - 8); f.ApplyStatus(Effect(StatusKind.Poison)); }
            s.AdvanceTime(5);
            Assert.AreEqual(CombatOutcome.Defeat, s.Outcome); Assert.IsNull(s.Actor); Assert.AreEqual(0, s.ReadyCount);
            Assert.AreEqual(8, s.Fighters[3].Health); // Enemy never receives a turn after final ally death.
            Assert.AreEqual(0, s.CompletedActions);
        }
        [Test] public void PoisonDeathSkipsOnlyVictimAndSurvivorContinues()
        {
            foreach (var p in data.participants) p.combatant.speed = 20;
            var s = new CombatSession(data); var mage = s.Fighters[1]; mage.Damage(102); mage.ApplyStatus(Effect(StatusKind.Poison));
            s.AdvanceTime(5); Assert.AreEqual("warrior", s.Actor.Definition.Id);
            Assert.AreEqual(2, s.ReadyCount); Assert.AreEqual(CombatOutcome.Ongoing, s.Outcome);
            Complete(s); Assert.AreEqual("shaman", s.Actor.Definition.Id);
        }
        [Test] public void SelfTonicKeepsTwoChargesThenExpiresAfterTwoActionsAndKeepsFixedRecovery()
        {
            var s = Solo("mage"); Next(s); var mage = s.Actor;
            Complete(s, CombatAction.HasteTonic, mage);
            Assert.AreEqual(2, Status(mage, StatusKind.Haste).Remaining); Assert.AreEqual(150, mage.EffectiveSpeed);
            Assert.AreEqual(0.8, mage.RecoveryRemaining, 1e-6);
            s.AdvanceTime(0.5); Assert.AreEqual(0, mage.Atb);
            s.AdvanceTime(0.5); Assert.AreEqual(30, mage.Atb, 1e-5);
            s.AdvanceTime(0.5); Assert.AreSame(mage, s.Actor);
            Complete(s); Assert.AreEqual(1, Status(mage, StatusKind.Haste).Remaining);
            Next(s); Complete(s); Assert.IsFalse(mage.HasStatus(StatusKind.Haste)); Assert.AreEqual(100, mage.EffectiveSpeed);
        }
        [Test] public void TonicPreservesOtherTargetsAccumulatedBarAndDecimalSpeed()
        {
            data.participants[0].combatant.speed = 21;
            data.participants[1].combatant.speed = 100;
            var s = new CombatSession(data); s.AdvanceTime(1); var warrior = s.Fighters[0];
            Assert.AreEqual(21, warrior.Atb); Complete(s, CombatAction.HasteTonic, warrior);
            Assert.AreEqual(21, warrior.Atb); Assert.AreEqual(31.5, warrior.EffectiveSpeed);
            s.AdvanceTime(0.1); Assert.AreEqual(24.15, warrior.Atb, 1e-6);
        }
        [Test] public void CorrosionKeepsDecimalDefenseRoundsFinalDamageAndExpiresOnSecondVictimAction()
        {
            data.participants[2].combatant.defense = 7;
            var s = Solo("mage"); Next(s); var ogre = s.Fighters[2];
            Complete(s, CombatAction.CorrosiveFlask, ogre); Assert.AreEqual(3.5, ogre.EffectiveDefense);
            Next(s); int hp = ogre.Health; Complete(s, CombatAction.Basic, ogre); Assert.AreEqual(hp - 21, ogre.Health);
            Assert.AreEqual(2, Status(ogre, StatusKind.Corrosion).Remaining);
            s = Solo("ogre"); ogre = s.Fighters[2]; ogre.ApplyStatus(Effect(StatusKind.Corrosion));
            Next(s); Complete(s); Assert.AreEqual(1, Status(ogre, StatusKind.Corrosion).Remaining); Assert.AreEqual(3.5, ogre.EffectiveDefense);
            Next(s); Complete(s); Assert.IsFalse(ogre.HasStatus(StatusKind.Corrosion)); Assert.AreEqual(7, ogre.EffectiveDefense);
        }
        [Test] public void RefreshDuringOwnActionGetsNewIdentityAndDoesNotConsumeNewCharges()
        {
            data.inventory.Single(e => e.item.command == CombatAction.HasteTonic).quantity = 2;
            var s = Solo("mage"); Next(s); var mage = s.Actor;
            Complete(s, CombatAction.HasteTonic, mage); var original = Status(mage, StatusKind.Haste);
            Next(s); Complete(s, CombatAction.HasteTonic, mage);
            var refreshed = Status(mage, StatusKind.Haste); Assert.AreNotEqual(original.ApplicationId, refreshed.ApplicationId);
            Assert.AreEqual(2, refreshed.Remaining); Assert.AreEqual(150, mage.EffectiveSpeed);
            Next(s); Complete(s); Assert.AreEqual(1, refreshed.Remaining);
            Next(s); Complete(s); Assert.IsFalse(mage.HasStatus(StatusKind.Haste));
        }
        [Test] public void DifferentEffectsCoexistAndRefreshPreservesPotencyAndOtherCounters()
        {
            var s = Solo("mage"); var mage = s.Fighters[1];
            foreach (StatusKind kind in Enum.GetValues(typeof(StatusKind))) mage.ApplyStatus(Effect(kind));
            Next(s); Complete(s); Assert.AreEqual(2, Status(mage, StatusKind.Poison).Remaining);
            Assert.AreEqual(1, Status(mage, StatusKind.Haste).Remaining); Assert.AreEqual(1, Status(mage, StatusKind.Corrosion).Remaining);
            var modified = data.inventory.Single(e => e.item.command == CombatAction.HasteTonic).item.appliedEffect;
            modified.multiplier = 4;
            mage.ApplyStatus(new EffectDefinition(modified));
            Assert.AreEqual(3, mage.Effects.Count); Assert.AreEqual(150, mage.EffectiveSpeed);
            Assert.AreEqual(2, Status(mage, StatusKind.Haste).Remaining);
            Assert.AreEqual(1, Status(mage, StatusKind.Corrosion).Remaining); Assert.AreEqual(2, Status(mage, StatusKind.Poison).Remaining);
        }
        [Test] public void AntidoteRemovesAtConfirmationAndDoesNotUndoOpeningDamageOrSpendTwice()
        {
            var s = Solo("mage"); var mage = s.Fighters[1]; mage.ApplyStatus(Effect(StatusKind.Poison)); Next(s);
            Assert.AreEqual(102, mage.Health); Assert.IsFalse(s.TryCommit(CombatAction.Antidote, s.Fighters[2], out _));
            Assert.AreEqual(2, s.QuantityFor(CombatAction.Antidote));
            Assert.IsTrue(s.TryCommit(CombatAction.Antidote, mage, out _)); Assert.IsFalse(mage.HasStatus(StatusKind.Poison));
            Assert.AreEqual(102, mage.Health); Assert.AreEqual(1, s.QuantityFor(CombatAction.Antidote));
            Assert.IsFalse(s.TryCommit(CombatAction.Antidote, mage, out _)); s.ApplyImpact(); s.FinishAction();
            Next(s); Assert.AreEqual(102, mage.Health); Assert.AreEqual("No hay objetivos válidos.", s.UnavailableReason(CombatAction.Antidote));
        }
        [Test] public void PauseDecisionAndExecutionNeverAgeStatusesAndRestartUsesNewSnapshots()
        {
            var s = Solo("mage"); Next(s); var mage = s.Actor; Complete(s, CombatAction.HasteTonic, mage);
            Next(s); int charges = Status(mage, StatusKind.Haste).Remaining;
            s.AdvanceTime(900); Assert.AreEqual(charges, Status(mage, StatusKind.Haste).Remaining);
            Assert.IsTrue(s.TryCommit(CombatAction.Basic, s.Fighters[2], out _)); s.IsPaused = true;
            s.AdvanceTime(900); Assert.IsFalse(s.ApplyImpact()); Assert.IsFalse(s.FinishAction());
            Assert.AreEqual(charges, Status(mage, StatusKind.Haste).Remaining);
            var source = data.inventory.Single(e => e.item.command == CombatAction.HasteTonic).item.appliedEffect;
            source.multiplier = 2; Assert.AreEqual(150, mage.EffectiveSpeed);
            var fresh = new CombatSession(data); Assert.IsTrue(fresh.Fighters.All(f => f.Effects.Count == 0));
            Assert.AreEqual(1, fresh.QuantityFor(CombatAction.HasteTonic));
            fresh.Fighters[1].ApplyStatus(fresh.ItemFor(CombatAction.HasteTonic).AppliedEffect);
            Assert.AreEqual(200, fresh.Fighters[1].EffectiveSpeed); Assert.AreEqual(2, source.duration);
        }
        [Test] public void PoisonSurvivesCastersDeathAndVictimsDeathClearsAllModifiers()
        {
            var s = Solo("shaman"); Next(s); Assert.IsTrue(s.CommitEnemyAction()); Assert.AreEqual(CombatAction.Poison, s.Pending.Kind);
            var target = s.Pending.Target; s.ApplyImpact(); s.FinishAction(); s.Fighters[3].Damage(999);
            Assert.IsTrue(target.HasStatus(StatusKind.Poison)); target.ApplyStatus(Effect(StatusKind.Haste)); target.ApplyStatus(Effect(StatusKind.Corrosion));
            target.Damage(999); Assert.AreEqual(0, target.Effects.Count);
            Assert.AreEqual(target.Definition.Speed, target.EffectiveSpeed); Assert.AreEqual(target.Definition.Defense, target.EffectiveDefense);
        }
        [TestCase(5, false)] [TestCase(6, true)] [TestCase(36, true)]
        public void ShamanSelectsOnlyUnpoisonedSurvivorsThenUsesBasicWhenAllPoisoned(int mana, bool poison)
        {
            data.participants[3].combatant.maxMana = mana;
            var s = Solo("shaman"); s.Fighters[0].ApplyStatus(Effect(StatusKind.Poison)); Next(s);
            Assert.IsTrue(s.CommitEnemyAction()); Assert.AreEqual(poison ? CombatAction.Poison : CombatAction.Basic, s.Pending.Kind);
            if (poison) Assert.AreSame(s.Fighters[1], s.Pending.Target);
            s.ApplyImpact(); s.FinishAction();
            s.Fighters[1].ApplyStatus(Effect(StatusKind.Poison)); Next(s);
            Assert.IsTrue(s.CommitEnemyAction()); Assert.AreEqual(CombatAction.Basic, s.Pending.Kind);
        }
        [Test] public void InvalidEffectTriggerIsRejectedBeforeCreatingSession()
        {
            data.inventory.Single(e => e.item.command == CombatAction.HasteTonic).item.appliedEffect.trigger = EffectTrigger.TurnStart;
            Assert.Throws<ArgumentException>(() => new CombatSession(data));
        }
    }
}
