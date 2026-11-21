using System.Linq;
using NUnit.Framework;

namespace UltimaGuardia.Tests
{
    public class CombatRulesTests
    {
        [TestCase(28, 1, 6, 22)]
        [TestCase(28, 1.8, 6, 44)]
        [TestCase(24, 1, 6, 18)]
        [TestCase(3, 1.5, 0, 5)]
        [TestCase(1, 1, 99, 1)]
        public void DamageUsesGddRounding(int attack, double potency, double defense, int expected) =>
            Assert.AreEqual(expected, CombatSession.CalculateDamage(attack, potency, defense));

        [Test]
        public void ResourcesAreClampedAndDeadCannotBeHealed()
        {
            var session = new CombatSession(1);
            var warrior = session.Actor;
            int events = 0;
            warrior.Changed += () => events++;
            Assert.AreEqual(0, warrior.Heal(50));
            Assert.AreEqual(0, warrior.Damage(-1));
            Assert.AreEqual(10, warrior.Damage(10));
            Assert.AreEqual(10, warrior.Heal(50));
            Assert.AreEqual(0, warrior.RestoreMana(999));
            Assert.AreEqual(160, warrior.Damage(999));
            Assert.IsFalse(warrior.IsAlive);
            Assert.AreEqual(0, warrior.Heal(999));
            Assert.AreEqual(0, warrior.RestoreMana(999));
            Assert.AreEqual(3, events);
        }

        [Test]
        public void InvalidTargetsAndDuplicateSignalsCannotSpendOrHitTwice()
        {
            var s = new CombatSession(1);
            var ogre = s.Fighters[2];
            Assert.IsFalse(s.TryCommit(CombatAction.HeavyStrike, s.Fighters[1], out _));
            Assert.IsFalse(s.TryCommit(CombatAction.Basic, new CombatSession().Fighters[2], out _));
            Assert.IsFalse(s.TryCommit(CombatAction.Potion, s.Actor, out _));
            Assert.AreEqual(30, s.Actor.Mana);
            Assert.AreEqual(3, s.Potions);
            Assert.IsTrue(s.TryCommit(CombatAction.HeavyStrike, ogre, out _));
            Assert.AreEqual(22, s.Actor.Mana);
            Assert.IsFalse(s.TryCommit(CombatAction.HeavyStrike, ogre, out _));
            Assert.IsFalse(s.FinishAction());
            Assert.AreEqual(240, ogre.Health);
            Assert.IsTrue(s.ApplyImpact());
            Assert.AreEqual(196, ogre.Health);
            Assert.IsFalse(s.ApplyImpact());
            Assert.AreEqual(196, ogre.Health);
            Assert.IsTrue(s.FinishAction());
            Assert.IsFalse(s.FinishAction());
            Assert.AreEqual("mage", s.Actor.Definition.Id);
        }

        [Test]
        public void PotionInventoryIsSharedLimitedAndEffectiveHealingIsCapped()
        {
            var s = new CombatSession(2);
            var warrior = s.Fighters[0];
            warrior.Damage(10);
            Assert.IsTrue(s.TryCommit(CombatAction.Potion, warrior, out _));
            Assert.AreEqual(2, s.Potions);
            Assert.AreEqual(30, warrior.Mana);
            s.ApplyImpact();
            Assert.AreEqual(10, s.Pending.EffectiveAmount);
            Assert.AreEqual(160, warrior.Health);
            s.FinishAction();
            s.Actor.Damage(60);
            Assert.IsTrue(s.TryCommit(CombatAction.Potion, s.Actor, out _));
            Assert.AreEqual(1, s.Potions);
            s.ApplyImpact(); s.FinishAction();
            AdvanceEnemyTurns(s);
            warrior.Damage(10);
            Assert.IsTrue(s.TryCommit(CombatAction.Potion, warrior, out _));
            s.ApplyImpact(); s.FinishAction();
            Assert.AreEqual(0, s.Potions);
            Assert.AreEqual("Sin unidades.", s.UnavailableReason(CombatAction.Potion));
        }

        [Test]
        public void InsufficientManaNeverConsumesATurnAndBasicRemainsAvailable()
        {
            var s = new CombatSession(3);
            for (int round = 0; round < 3; round++)
            {
                Assert.IsTrue(s.TryCommit(CombatAction.HeavyStrike, s.Fighters[2], out _));
                s.ApplyImpact(); s.FinishAction();
                Assert.IsTrue(s.TryCommit(CombatAction.Basic, s.Fighters[2], out _));
                s.ApplyImpact(); s.FinishAction();
                AdvanceEnemyTurns(s);
            }
            Assert.AreEqual(6, s.Actor.Mana);
            int completed = s.CompletedActions;
            Assert.IsFalse(s.TryCommit(CombatAction.HeavyStrike, s.Fighters[3], out string reason));
            Assert.AreEqual("Maná insuficiente.", reason);
            Assert.AreEqual(completed, s.CompletedActions);
            Assert.IsTrue(s.TryCommit(CombatAction.Basic, s.Fighters[3], out _));
        }

        [Test]
        public void DeadTargetsAndTurnsAreSkippedAndOneSurvivorCanAct()
        {
            var s = new CombatSession(4);
            s.Fighters[1].Damage(999);
            s.Fighters[2].Damage(999);
            CollectionAssert.AreEqual(new[] { s.Fighters[3] }, s.ResolveTargets(s.Actor, TargetMode.AllEnemies));
            CollectionAssert.AreEqual(new[] { s.Actor }, s.ResolveTargets(s.Actor, TargetMode.Ally));
            Assert.IsFalse(s.TryCommit(CombatAction.Basic, s.Fighters[2], out _));
            Assert.IsTrue(s.TryCommit(CombatAction.Basic, s.Fighters[3], out _));
            s.ApplyImpact(); s.FinishAction();
            Assert.AreEqual("shaman", s.Actor.Definition.Id);
            Assert.IsTrue(s.CommitEnemyAction());
            Assert.AreSame(s.Fighters[0], s.Pending.Target);
            s.ApplyImpact(); s.FinishAction();
            Assert.AreEqual("warrior", s.Actor.Definition.Id);
        }

        [Test]
        public void LastEnemyDeathEndsCombatBeforeAnotherCommand()
        {
            var s = new CombatSession(5);
            s.Fighters[2].Damage(999);
            s.Fighters[3].Damage(129);
            s.TryCommit(CombatAction.Basic, s.Fighters[3], out _);
            s.ApplyImpact();
            Assert.AreEqual(CombatOutcome.Victory, s.Outcome);
            Assert.IsFalse(s.TryCommit(CombatAction.Potion, s.Actor, out _));
            s.FinishAction();
            Assert.IsNull(s.Pending);
            Assert.IsFalse(s.CommitEnemyAction());
        }

        private static void AdvanceEnemyTurns(CombatSession s)
        {
            while (s.Actor.Definition.Team == CombatTeam.Enemies && s.Outcome == CombatOutcome.Ongoing)
            { Assert.IsTrue(s.CommitEnemyAction()); s.ApplyImpact(); s.FinishAction(); }
        }
    }
}
