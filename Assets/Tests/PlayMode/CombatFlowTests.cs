using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UltimaGuardia.Tests
{
    public class CombatFlowTests
    {
        private Keyboard keyboard;
        private Gamepad gamepad;
        private bool wasBackground;
        private InputSettings.BackgroundBehavior previousBehavior;
        private CombatPrototype Battle => Object.FindFirstObjectByType<CombatPrototype>();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            wasBackground = Application.runInBackground;
            previousBehavior = InputSystem.settings.backgroundBehavior;
            Application.runInBackground = true;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            keyboard = InputSystem.AddDevice<Keyboard>();
            gamepad = InputSystem.AddDevice<Gamepad>();
            yield return SceneManager.LoadSceneAsync("Combat");
            yield return null; yield return null;
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            InputSystem.RemoveDevice(keyboard); InputSystem.RemoveDevice(gamepad);
            InputSystem.settings.backgroundBehavior = previousBehavior;
            Application.runInBackground = wasBackground;
            yield return null;
        }

        [UnityTest]
        public IEnumerator CancellationPauseRepeatedConfirmAndRestartPreserveRules()
        {
            yield return Capture("arena-1280x720", 1280, 720);
            yield return Capture("arena-1920x1080", 1920, 1080);
            yield return Press(Key.DownArrow);
            yield return Press(Key.Enter);
            Assert.AreEqual(PrototypePhase.Targets, Battle.Phase);
            Assert.AreEqual(30, Battle.Session.Actor.Mana);
            yield return Press(Key.RightArrow);
            yield return Capture("target", 1280, 720);
            yield return Press(Key.Escape);
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
            Assert.IsFalse(Battle.IsPaused);
            Assert.AreEqual(30, Battle.Session.Actor.Mana);
            Assert.AreEqual(3, Battle.Session.Potions);
            yield return Press(Key.Enter); // reopen targets
            var target = Battle.SelectedTarget;
            yield return Press(Key.Enter); // commit heavy
            Assert.AreEqual(PrototypePhase.Executing, Battle.Phase);
            Assert.AreEqual(22, Battle.Session.Actor.Mana);
            yield return Press(Key.Enter); // ignored during execution
            yield return Press(Key.Escape); // before impact
            Assert.IsTrue(Battle.IsPaused);
            float elapsed = Battle.ActionTime;
            int health = target.Health;
            yield return new WaitForSecondsRealtime(0.45f);
            Assert.AreEqual(elapsed, Battle.ActionTime);
            Assert.AreEqual(health, target.Health);
            yield return Capture("pause", 1280, 720);
            yield return Press(Key.Escape);
            while (Battle.ActionTime < 0.4f) yield return null;
            Assert.AreEqual(health - CombatSession.CalculateDamage(28, 1.8, target.Definition.Defense), target.Health);
            yield return Press(Key.Escape); // after impact
            int damaged = target.Health;
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.AreEqual(damaged, target.Health);
            yield return Press(Key.DownArrow); // restart from pause
            yield return Press(Key.Enter);
            AssertFreshBattle();
            yield return Press(Key.Enter); // target selection
            yield return Press(Key.Escape);
            yield return Press(Key.Escape);
            yield return Press(Key.DownArrow);
            yield return Press(Key.DownArrow);
            yield return Press(Key.Enter);
            Assert.AreEqual("MainMenu", SceneManager.GetActiveScene().name);
        }

        [UnityTest]
        public IEnumerator KeyboardWinsNormalEncounterAndRestartsFromResult()
        {
            yield return PlayToResult(false);
            Assert.AreEqual(CombatOutcome.Victory, Battle.Session.Outcome);
            yield return Capture("victory", 1280, 720);
            TestContext.WriteLine("Normal keyboard victory: " + Battle.Session.CompletedActions + " actions, potions " + Battle.Session.Potions);
            yield return Press(Key.Enter);
            AssertFreshBattle();
        }

        [UnityTest]
        public IEnumerator VirtualGamepadWinsThenLosesLowHealthScenarioAndRestarts()
        {
            // Stick navigation and cancellation before the full dpad/button route.
            InputSystem.QueueStateEvent(gamepad, new GamepadState { leftStick = Vector2.down });
            yield return null; yield return null;
            InputSystem.QueueStateEvent(gamepad, new GamepadState());
            yield return null;
            Assert.AreEqual(1, Battle.SelectedCommand);
            yield return Pad(GamepadButton.South);
            yield return Pad(GamepadButton.East);
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
            yield return Pad(GamepadButton.DpadUp);
            yield return PlayToResult(true);
            Assert.AreEqual(CombatOutcome.Victory, Battle.Session.Outcome);
            TestContext.WriteLine("Normal virtual gamepad victory: " + Battle.Session.CompletedActions + " actions, potions " + Battle.Session.Potions);
            yield return Pad(GamepadButton.South);
            AssertFreshBattle();
            // Explicit boundary fixture, not a claim about natural balance or physical hardware.
            foreach (var ally in Battle.Session.Fighters.Where(f => f.Definition.Team == CombatTeam.Allies))
                ally.Damage(ally.Health - 1);
            yield return PlayToResult(true, false);
            Assert.AreEqual(CombatOutcome.Defeat, Battle.Session.Outcome);
            yield return Capture("defeat-forced-low-health", 1280, 720);
            yield return Pad(GamepadButton.South);
            AssertFreshBattle();
            yield return Pad(GamepadButton.Start);
            Assert.IsTrue(Battle.IsPaused);
            yield return Pad(GamepadButton.East);
            Assert.IsFalse(Battle.IsPaused);
        }

        private IEnumerator PlayToResult(bool pad, bool useResources = true)
        {
            float deadline = Time.realtimeSinceStartup + 120;
            while (Battle.Phase != PrototypePhase.Result && Time.realtimeSinceStartup < deadline)
            {
                if (Battle.Phase == PrototypePhase.Commands && Battle.Session.Actor.Definition.Team == CombatTeam.Allies)
                {
                    var s = Battle.Session;
                    bool potion = useResources && s.Potions > 0 && s.Fighters.Any(f => f.Definition.Team == CombatTeam.Allies && f.IsAlive && f.Health <= f.Definition.MaxHealth - 50);
                    bool heavy = useResources && !potion && s.Actor.Definition.Id == "warrior" && s.Actor.Mana >= 8;
                    int desired = potion ? (s.Actor.Definition.Id == "warrior" ? 2 : 1) : heavy ? 1 : 0;
                    while (Battle.SelectedCommand != desired) yield return Direction(pad, false);
                    yield return Confirm(pad);
                    Assert.AreEqual(PrototypePhase.Targets, Battle.Phase);
                    var wanted = potion ? s.Fighters.Where(f => f.Definition.Team == CombatTeam.Allies && f.IsAlive && f.Health < f.Definition.MaxHealth)
                        .OrderBy(f => (float)f.Health / f.Definition.MaxHealth).First()
                        : s.Fighters.Where(f => f.Definition.Team == CombatTeam.Enemies && f.IsAlive).OrderBy(f => f.Definition.Id == "shaman" ? 0 : 1).First();
                    int steps = 0;
                    while (Battle.SelectedTarget != wanted && steps++ < 4) yield return Direction(pad, true);
                    Assert.AreSame(wanted, Battle.SelectedTarget);
                    yield return Confirm(pad);
                    Assert.AreEqual(PrototypePhase.Executing, Battle.Phase);
                }
                yield return null;
            }
            Assert.AreEqual(PrototypePhase.Result, Battle.Phase, "Combat must finish before deadline.");
            Assert.IsNull(Battle.Session.Pending);
        }

        private void AssertFreshBattle()
        {
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase);
            Assert.IsFalse(Battle.IsPaused);
            Assert.IsNull(Battle.Session.Pending);
            Assert.AreEqual(3, Battle.Session.Potions);
            Assert.AreEqual(0, Battle.Session.CompletedActions);
            Assert.AreEqual("warrior", Battle.Session.Actor.Definition.Id);
            foreach (var f in Battle.Session.Fighters)
            { Assert.AreEqual(f.Definition.MaxHealth, f.Health); Assert.AreEqual(f.Definition.MaxMana, f.Mana); }
        }

        private IEnumerator Direction(bool pad, bool horizontal) => pad ? Pad(horizontal ? GamepadButton.DpadRight : GamepadButton.DpadDown) : Press(horizontal ? Key.RightArrow : Key.DownArrow);
        private IEnumerator Confirm(bool pad) => pad ? Pad(GamepadButton.South) : Press(Key.Enter);
        private IEnumerator Press(Key key)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
            yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null; yield return null;
        }
        private IEnumerator Pad(GamepadButton button)
        {
            InputSystem.QueueStateEvent(gamepad, new GamepadState().WithButton(button));
            yield return null; yield return null;
            InputSystem.QueueStateEvent(gamepad, new GamepadState());
            yield return null; yield return null;
        }

        private IEnumerator Capture(string name, int width, int height)
        {
            yield return null;
            var camera = Camera.main;
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var oldMode = canvas.renderMode; var oldCamera = canvas.worldCamera;
            float oldDistance = canvas.planeDistance;
            var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active;
            var render = RenderTexture.GetTemporary(width, height, 24);
            Texture2D image = null;
            try
            {
                camera.targetTexture = render;
                canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
                yield return null;
                Canvas.ForceUpdateCanvases();
                camera.Render(); RenderTexture.active = render;
                image = new Texture2D(width, height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
                var directory = Path.Combine(Application.dataPath, "../Documentation/Evidence/H01");
                Directory.CreateDirectory(directory);
                File.WriteAllBytes(Path.Combine(directory, name + ".png"), image.EncodeToPNG());
            }
            finally
            {
                canvas.renderMode = oldMode; canvas.worldCamera = oldCamera; canvas.planeDistance = oldDistance;
                camera.targetTexture = oldTarget; RenderTexture.active = oldActive;
                if (image != null) Object.Destroy(image);
                RenderTexture.ReleaseTemporary(render);
            }
        }
    }
}
