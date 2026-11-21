using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace UltimaGuardia.Tests
{
    public class AnimationPresentationTests
    {
        private CombatPrototype battle;
        private CombatantView[] views;
        private readonly List<Object> owned = new List<Object>();
        private bool previousBackground;
        private string evidence => Path.Combine(Application.dataPath, "../Documentation/Evidence/H06");

        [UnitySetUp]
        public IEnumerator Setup()
        {
            previousBackground = Application.runInBackground; Application.runInBackground = true;
            yield return SceneManager.LoadSceneAsync("Combat"); yield return null;
            battle = Object.FindFirstObjectByType<CombatPrototype>(); battle.enabled = false;
            views = Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None);
            Directory.CreateDirectory(evidence);
        }
        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            if (battle != null) battle.Presentation.Cancel();
            foreach (var asset in owned) Object.DestroyImmediate(asset); owned.Clear();
            Application.runInBackground = previousBackground;
            yield return null;
        }
        private CombatSession Fixture(string actorId)
        {
            var data = Object.Instantiate(Resources.Load<EncounterData>("Encounters/MainEncounter")); owned.Add(data);
            data.participants = data.participants.Select(slot => {
                var fighter = Object.Instantiate(slot.combatant); owned.Add(fighter); fighter.speed = fighter.id == actorId ? 100 : 1;
                return new EncounterSlot { combatant = fighter, position = slot.position, rotation = slot.rotation };
            }).ToArray();
            battle.ConfigureEncounter(data, null); battle.RestartCombat();
            battle.Session.AdvanceTime(1);
            Assert.AreEqual(actorId, battle.Session.Actor.Definition.Id);
            return battle.Session;
        }
        private CharacterAnimationPlayer Player(string id) => views.Single(v => v.combatantId == id).GetComponentInChildren<CharacterAnimationPlayer>();
        private void Tick(CombatSession session, float dt)
        {
            battle.Presentation.Tick(dt);
            foreach (var fighter in session.Fighters) Player(fighter.Definition.Id).Tick(session.IsPaused ? 0 : dt, fighter);
        }

        [UnityTest]
        public IEnumerator EveryActionAndBothAlliesItemsAnimateWithSingleImpactAndFixedRecovery()
        {
            var rows = new List<string>();
            foreach (var actorId in new[] { "warrior", "mage", "ogre", "shaman" })
            {
                var source = Resources.Load<EncounterData>("Encounters/MainEncounter").participants.Single(p => p.combatant.id == actorId).combatant;
                var commands = source.actions.Select(a => a.command).Concat(source.team == CombatTeam.Allies ?
                    new[] { CombatAction.Potion, CombatAction.Ether, CombatAction.HasteTonic, CombatAction.CorrosiveFlask, CombatAction.Antidote } : new CombatAction[0]);
                foreach (var command in commands)
                {
                    var session = Fixture(actorId); var actor = session.Actor;
                    var ally = session.Fighters.First(f => f.Definition.Team == actor.Definition.Team && f != actor);
                    if (command == CombatAction.Heal || command == CombatAction.Potion || command == CombatAction.DarkHeal) ally.Damage(50);
                    if (command == CombatAction.Ether)
                    {
                        // Prepare missing mana through an actual committed cast in a separate state,
                        // then use the current actor as Ether's valid target in this fixture.
                        var spend = actor.Definition.Actions.First(a => a.ManaCost > 0 && a.Effect == ActionEffect.Damage);
                        Assert.IsTrue(session.TryCommit(spend.Command, session.TargetsFor(spend.Command)[0], out var unused));
                        session.ApplyImpact(); session.FinishAction();
                        for (int i = 0; session.Actor != actor && i < 300; i++)
                        {
                            if (session.Actor != null) { session.TryCommit(CombatAction.Basic, session.TargetsFor(CombatAction.Basic)[0], out unused); session.ApplyImpact(); session.FinishAction(); }
                            else session.AdvanceTime(.1);
                        }
                        Assert.AreSame(actor, session.Actor);
                    }
                    if (command == CombatAction.Antidote) ally.ApplyStatus(session.ItemFor(command).AppliedEffect);
                    var target = session.TargetsFor(command)[0];
                    Assert.IsTrue(session.TryCommit(command, target, out var error), actorId + "/" + command + ": " + error);
                    var pending = session.Pending; int mana = actor.Mana, quantity = session.QuantityFor(command);
                    var roots = views.Select(v => v.transform.position).ToArray();
                    battle.Presentation.Begin(session);
                    Assert.AreEqual(pending.Item == null ? command.ToString() : "Item", Player(actorId).CurrentState);
                    var rig = Player(actorId).GetComponent<CharacterRig>();
                    var rest = rig.restRotations.ToArray();
                    Tick(session, .2f);
                    Assert.IsFalse(pending.ImpactApplied); Assert.AreEqual(0, pending.Impacts.Count);
                    Assert.IsTrue(rig.joints.Where((j, i) => Quaternion.Angle(j.localRotation, rest[i]) > 1).Any(), actorId + "/" + command + ": visible pose");
                    if (Player(actorId).animations.Find(command)?.projectile == true)
                    {
                        Assert.IsTrue(battle.Presentation.HasProjectile);
                        Assert.AreEqual(target.Definition.Id, battle.Presentation.ConfirmedTargetId);
                    }
                    yield return null;
                    Capture(actorId + "-" + command + "-anticipation", 1280, 720);
                    int steps = 0;
                    while (!pending.ImpactApplied && steps++ < 200) Tick(session, .01f);
                    Assert.IsTrue(pending.ImpactApplied);
                    Assert.AreEqual(pending.Targets.Count, pending.Impacts.Count);
                    Assert.IsFalse(session.ApplyImpact(), "Duplicate impact must be rejected");
                    var health = session.Fighters.Select(f => f.Health).ToArray();
                    session.IsPaused = true;
                    float paused = battle.Presentation.Time;
                    var jointPositions = rig.joints.Select(j => j.localPosition).ToArray();
                    var jointRotations = rig.joints.Select(j => j.localRotation).ToArray();
                    Tick(session, 10);
                    Assert.AreEqual(paused, battle.Presentation.Time);
                    CollectionAssert.AreEqual(jointPositions, rig.joints.Select(j => j.localPosition));
                    CollectionAssert.AreEqual(jointRotations, rig.joints.Select(j => j.localRotation));
                    session.IsPaused = false;
                    Tick(session, 3);
                    CollectionAssert.AreEqual(health, session.Fighters.Select(f => f.Health));
                    Assert.AreEqual(command == CombatAction.Ether && target == actor ? Mathf.Min(actor.Definition.MaxMana, mana + 20) : mana, actor.Mana);
                    Assert.AreEqual(quantity, session.QuantityFor(command));
                    double recovery = pending.Recovery;
                    Assert.IsTrue(session.FinishAction());
                    Assert.AreEqual(recovery, actor.RecoveryRemaining);
                    Assert.AreEqual("Idle", Player(actorId).CurrentState);
                    for (int i = 0; i < views.Length; i++) Assert.AreEqual(roots[i], views[i].transform.position);
                    for (int i = 0; i < rig.joints.Length; i++)
                        Assert.Less(Vector3.Distance(rig.restPositions[i], rig.joints[i].localPosition), .0001f);
                    rows.Add(actorId + "/" + command + ": passed, impact=" + battle.Presentation.ImpactTime + ", recovery=" + recovery);
                    yield return null;
                }
            }
            File.WriteAllLines(Path.Combine(evidence, "action-matrix.txt"), rows);
            Assert.AreEqual(20, rows.Count, "Ten actions plus five items for each ally");
        }

        [UnityTest]
        public IEnumerator ProjectilePauseCancelAndRestartCannotAffectNextSession()
        {
            var session = Fixture("mage"); var target = session.TargetsFor(CombatAction.Basic)[0];
            session.TryCommit(CombatAction.Basic, target, out _); var pending = session.Pending;
            battle.Presentation.Begin(session); Tick(session, .2f);
            Assert.IsTrue(battle.Presentation.HasProjectile);
            var position = battle.Presentation.ProjectilePosition;
            session.IsPaused = true; Tick(session, 50);
            Assert.AreEqual(position, battle.Presentation.ProjectilePosition);
            Assert.IsFalse(pending.ImpactApplied);
            battle.RestartCombat();
            Assert.IsFalse(battle.Presentation.HasProjectile);
            var health = battle.Session.Fighters.Select(f => f.Health).ToArray();
            Tick(battle.Session, 5);
            CollectionAssert.AreEqual(health, battle.Session.Fighters.Select(f => f.Health));
            Assert.IsFalse(pending.ImpactApplied);
            yield return null;
            Assert.IsFalse(Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Any(r => r.name == "H06_ConfirmedProjectile"));
            battle.Presentation.Begin(CommitBasic(battle.Session));
            yield return SceneManager.LoadSceneAsync("MainMenu"); yield return null;
            Assert.AreEqual(0, Object.FindObjectsByType<CharacterAnimationPlayer>(FindObjectsSortMode.None).Length);
            Assert.IsFalse(Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Any(r => r.name == "H06_ConfirmedProjectile"));
            // TearDown must not dereference the destroyed scene owner.
            battle = Object.FindFirstObjectByType<CombatPrototype>();
        }
        private CombatSession CommitBasic(CombatSession session)
        { session.AdvanceTime(1); session.TryCommit(CombatAction.Basic, session.TargetsFor(CombatAction.Basic)[0], out _); return session; }

        [UnityTest]
        public IEnumerator DeathHoldsFinalPoseRejectsNewActionsAndRestartRestoresAllJoints()
        {
            var session = Fixture("mage");
            foreach (var fighter in session.Fighters)
            {
                var player = Player(fighter.Definition.Id); var rig = player.GetComponent<CharacterRig>();
                fighter.Damage(999); player.Tick(.5f, fighter);
                Assert.AreEqual("Death", player.CurrentState);
                Assert.Greater(Quaternion.Angle(rig.hips.localRotation, rig.restRotations[0]), 75, "Death must actually rotate the hips");
                var pose = rig.joints.Select(j => j.localRotation).ToArray();
                player.EndAction(); player.Tick(20, fighter);
                Assert.AreEqual("Death", player.CurrentState);
                CollectionAssert.AreEqual(pose, rig.joints.Select(j => j.localRotation));
            }
            yield return null;
            Capture("all-deaths-1920x1080", 1920, 1080);
            battle.RestartCombat();
            foreach (var fighter in battle.Session.Fighters)
            {
                var player = Player(fighter.Definition.Id); var rig = player.GetComponent<CharacterRig>();
                Assert.AreEqual("Idle", player.CurrentState); Assert.IsFalse(player.IsDead);
                for (int i = 0; i < rig.joints.Length; i++)
                {
                    Assert.Less(Quaternion.Angle(rig.joints[i].localRotation, rig.restRotations[i]), .01f);
                    Assert.Less(Vector3.Distance(rig.joints[i].localPosition, rig.restPositions[i]), .0001f);
                }
            }
            yield return null;
        }

        private void Capture(string name, int width, int height)
        {
            var camera = Camera.main; var canvas = Object.FindFirstObjectByType<Canvas>();
            bool enabled = canvas.enabled; canvas.enabled = false;
            var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active; float oldAspect = camera.aspect;
            var rt = RenderTexture.GetTemporary(width, height, 24); Texture2D image = null;
            try
            {
                camera.targetTexture = rt; camera.aspect = (float)width / height; camera.Render(); RenderTexture.active = rt;
                image = new Texture2D(width, height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
                File.WriteAllBytes(Path.Combine(evidence, name + ".png"), image.EncodeToPNG());
            }
            finally
            { canvas.enabled = enabled; camera.targetTexture = oldTarget; camera.aspect = oldAspect; RenderTexture.active = oldActive; if (image != null) Object.DestroyImmediate(image); RenderTexture.ReleaseTemporary(rt); }
        }
    }
}
