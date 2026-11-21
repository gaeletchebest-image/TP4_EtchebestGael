using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace UltimaGuardia.Tests
{
    public class FinalInterfaceTests
    {
        private Keyboard keyboard;
        private Gamepad gamepad;
        private bool background;
        private float volume;
        private int sessionVolume;
        private InputSettings.BackgroundBehavior behavior;
#if UNITY_EDITOR
        private InputSettings.EditorInputBehaviorInPlayMode editorInput;
#endif
        private CombatPrototype Battle => Object.FindFirstObjectByType<CombatPrototype>();
        private FoundationMenu Main => Object.FindFirstObjectByType<FoundationMenu>();
        [UnitySetUp] public IEnumerator SetUp()
        {
            background = Application.runInBackground; behavior = InputSystem.settings.backgroundBehavior; volume = AudioListener.volume; sessionVolume = SessionOptions.Volume;
            Application.runInBackground = true; InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
            editorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            keyboard = InputSystem.AddDevice<Keyboard>(); gamepad = InputSystem.AddDevice<Gamepad>();
            yield return SceneManager.LoadSceneAsync("MainMenu"); yield return null; yield return null;
        }
        [UnityTearDown] public IEnumerator TearDown()
        {
            InputSystem.RemoveDevice(keyboard); InputSystem.RemoveDevice(gamepad);
            Application.runInBackground = background; InputSystem.settings.backgroundBehavior = behavior; SessionOptions.SetVolume(sessionVolume); AudioListener.volume = volume;
#if UNITY_EDITOR
            InputSystem.settings.editorInputBehaviorInPlayMode = editorInput;
#endif
            yield return null;
        }
        [UnityTest] public IEnumerator KeyboardTraversesFinalMenusHelpVolumeAndConfirmations() { yield return MenuRoute(false); }
        [UnityTest] public IEnumerator GamepadTraversesFinalMenusHelpVolumeAndConfirmations() { yield return MenuRoute(true); }
        private IEnumerator MenuRoute(bool pad)
        {
            string device = pad ? "gamepad" : "keyboard";
            yield return Capture("main-" + device, 1280, 720);
            yield return Capture("main-" + device, 1920, 1080);
            yield return Press(pad, Key.UpArrow); AssertSelection("QuitButton");
            yield return Press(pad, Key.DownArrow); AssertSelection("PlayButton");
            yield return Press(pad, Key.DownArrow); yield return Press(pad, Key.Enter);
            Assert.IsTrue(Main.ControlsOpen); AssertSelection("ControlsBackButton");
            yield return Press(pad, Key.LeftArrow); Assert.AreEqual(3, Main.HelpPage);
            yield return Press(pad, Key.RightArrow); Assert.AreEqual(0, Main.HelpPage);
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(i, Main.HelpPage);
                yield return Capture("help-" + i + "-" + device, 1280, 720);
                yield return Capture("help-" + i + "-" + device, 1920, 1080);
                yield return Press(pad, Key.RightArrow);
            }
            yield return Press(pad, Key.Escape); AssertSelection("ControlsButton");
            yield return Press(pad, Key.DownArrow); AssertSelection("VolumeButton"); yield return Press(pad, Key.Enter);
            Assert.IsTrue(Main.VolumeOpen);
            for (int i = 0; i < 12; i++) yield return Press(pad, Key.LeftArrow);
            Assert.AreEqual(0, SessionOptions.Volume); Assert.AreEqual(0, AudioListener.volume);
            yield return Capture("volume-zero-" + device, 1280, 720);
            for (int i = 0; i < 12; i++) yield return Press(pad, Key.RightArrow);
            Assert.AreEqual(100, SessionOptions.Volume); Assert.AreEqual(1, AudioListener.volume);
            yield return Press(pad, Key.LeftArrow); Assert.AreEqual(90, SessionOptions.Volume);
            yield return Press(pad, Key.Escape); AssertSelection("VolumeButton");
            yield return Press(pad, Key.UpArrow); yield return Press(pad, Key.UpArrow); yield return Press(pad, Key.Enter);
            yield return WaitScene("Combat"); Assert.AreEqual(90, SessionOptions.Volume);
            yield return Press(pad, Key.Escape);
            if (pad && !Battle.IsPaused) { yield return Pad(GamepadButton.Start); }
            Assert.IsTrue(Battle.IsPaused); double clock = Battle.Session.ActiveTime;
            yield return Press(pad, Key.DownArrow); yield return Press(pad, Key.Enter); Assert.AreEqual(1, Battle.PausePage);
            for (int i = 0; i < 4; i++)
            {
                yield return Capture("pause-help-" + i + "-" + device, 1280, 720);
                yield return Press(pad, Key.RightArrow);
            }
            yield return Press(pad, Key.Escape); Assert.IsTrue(Battle.IsPaused); Assert.AreEqual(1, Battle.OverlaySelection);
            yield return Press(pad, Key.DownArrow); yield return Press(pad, Key.Enter); Assert.AreEqual(2, Battle.PausePage);
            yield return Press(pad, Key.LeftArrow); Assert.AreEqual(80, SessionOptions.Volume);
            yield return Capture("pause-volume-" + device, 1920, 1080);
            yield return Press(pad, Key.Escape); Assert.AreEqual(2, Battle.OverlaySelection);
            yield return Press(pad, Key.DownArrow); yield return Press(pad, Key.Enter); Assert.AreEqual(3, Battle.PausePage);
            Assert.AreEqual(0, Battle.ConfirmationSelection);
            var session = Battle.Session;
            yield return Capture("restart-confirm-" + device, 1280, 720);
            yield return Press(pad, Key.Enter); Assert.AreSame(session, Battle.Session); Assert.AreEqual(0, Battle.PausePage);
            yield return Press(pad, Key.Enter); yield return Press(pad, Key.Escape); Assert.AreEqual(3, Battle.OverlaySelection);
            yield return Press(pad, Key.Enter); yield return Press(pad, Key.DownArrow); yield return Press(pad, Key.Enter);
            Assert.AreNotSame(session, Battle.Session); Assert.IsFalse(Battle.IsPaused); Assert.AreEqual(80, SessionOptions.Volume);
            Assert.AreEqual(clock, session.ActiveTime, "Nested pause pages must freeze combat");
            yield return pad ? Pad(GamepadButton.Start) : Press(false, Key.Escape);
            for (int i = 0; i < 4; i++) yield return Press(pad, Key.DownArrow);
            yield return Press(pad, Key.Enter); Assert.AreEqual(3, Battle.PausePage);
            yield return Capture("leave-confirm-" + device, 1920, 1080);
            yield return Press(pad, Key.Escape); Assert.AreEqual(4, Battle.OverlaySelection);
            yield return Press(pad, Key.Enter); yield return Press(pad, Key.DownArrow); yield return Press(pad, Key.Enter);
            yield return WaitScene("MainMenu"); Assert.AreEqual(80, SessionOptions.Volume); AssertSelection("PlayButton");
        }
        [UnityTest] public IEnumerator FinalHudShowsRestrictionsGroupStatesAndEffectiveFeedback()
        {
            yield return SceneManager.LoadSceneAsync("Combat"); yield return null; yield return null;
            Battle.Session.AdvanceTime(4); yield return null;
            Assert.AreEqual("mage", Battle.Session.Actor.Definition.Id);
            var mage = Battle.Session.Actor;
            foreach (var e in new[] { "poison", "haste", "corrosion" })
                mage.ApplyStatus(new EffectDefinition(Resources.Load<EffectData>("Encounters/" + e)));
            yield return Capture("hud-states", 1280, 720); yield return Capture("hud-states", 1920, 1080);
            yield return Press(false, Key.DownArrow); yield return Press(false, Key.DownArrow); yield return Press(false, Key.Enter);
            for (int i = 0; i < 5; i++)
            {
                yield return Capture("item-" + i, 1280, 720); yield return Press(false, Key.DownArrow);
            }
            // Healthy party cannot consume a potion. Description stays visible.
            int count = Battle.Session.Potions; yield return Press(false, Key.Enter);
            Assert.AreEqual(PrototypePhase.Commands, Battle.Phase); Assert.AreEqual(count, Battle.Session.Potions);
            yield return Press(false, Key.Escape); yield return Press(false, Key.UpArrow); yield return Press(false, Key.Enter);
            yield return Press(false, Key.DownArrow); yield return Press(false, Key.Enter);
            Assert.AreEqual(PrototypePhase.Targets, Battle.Phase);
            Assert.AreEqual(2, Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None).Count(v => v.targetMarker.activeSelf));
            yield return Capture("storm-group", 1280, 720);
            foreach (var v in Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None).Where(v => v.targetMarker.activeSelf))
                Assert.AreEqual(Battle.Session.Fighters.Single(f => f.Definition.Id == v.combatantId).Definition.Name, v.nameLabel.text);
            yield return Press(false, Key.Escape); yield return Press(false, Key.Escape);
            var ogre = Battle.Session.Fighters.First(f => f.Definition.Id == "ogre"); ogre.Damage(ogre.Health - 1);
            Assert.IsTrue(Battle.Session.TryCommit(CombatAction.Basic, ogre, out _)); Assert.IsTrue(Battle.Session.ApplyImpact());
            Assert.IsFalse(Battle.Session.ApplyImpact()); yield return null;
            var hud = Object.FindFirstObjectByType<FinalCombatHud>(); Assert.AreEqual(1, hud.VisibleNumbers);
            Assert.IsTrue(Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None).All(v => !v.nameLabel.gameObject.activeSelf));
            StringAssert.Contains("−1 VIDA", string.Join(" ", Object.FindObjectsByType<Text>(FindObjectsSortMode.None).Select(t => t.text)));
            yield return Capture("effective-damage", 1280, 720);
            Battle.RestartCombat(); yield return null; Assert.AreEqual(0, hud.VisibleNumbers); Assert.AreEqual(1, Battle.Session.CombatLog.Count);
            // Outcome requires no extra confirmation, and fresh attempts restore all cards.
            foreach (var f in Battle.Session.Fighters.Where(f => f.Definition.Team == CombatTeam.Enemies)) f.Damage(999);
            yield return null; yield return Capture("victory", 1280, 720); yield return Capture("victory", 1920, 1080);
            yield return Press(false, Key.Enter); Assert.AreEqual(CombatOutcome.Ongoing, Battle.Session.Outcome);
            foreach (var f in Battle.Session.Fighters.Where(f => f.Definition.Team == CombatTeam.Allies)) f.Damage(999);
            yield return null; yield return Capture("defeat", 1280, 720); yield return Capture("defeat", 1920, 1080);
        }
        [UnityTest] public IEnumerator RecoveryAndPoisonNumbersUseEffectiveResourcesAndRestartClearsSubscriptions()
        {
            yield return SceneManager.LoadSceneAsync("Combat"); yield return null; yield return null;
            var hud = Object.FindFirstObjectByType<FinalCombatHud>(); var s = Battle.Session;
            s.AdvanceTime(4); var mage = s.Actor; mage.Damage(5);
            Assert.IsTrue(s.TryCommit(CombatAction.Heal, mage, out _)); Assert.IsTrue(s.ApplyImpact());
            Assert.AreEqual(5, s.Pending.EffectiveAmount); Assert.AreEqual(1, hud.VisibleNumbers);
            yield return Capture("effective-heal", 1280, 720);
            Assert.IsTrue(Object.FindObjectsByType<Text>(FindObjectsSortMode.None).Any(t => t.text == "+5 VIDA"));
            Battle.RestartCombat(); Assert.AreEqual(0, hud.VisibleNumbers); s = Battle.Session;
            s.AdvanceTime(4); mage = s.Actor;
            Assert.IsTrue(s.TryCommit(CombatAction.ArcaneStorm, s.TargetsFor(CombatAction.ArcaneStorm)[0], out _));
            Assert.IsTrue(s.ApplyImpact()); Assert.IsTrue(s.FinishAction()); s.AdvanceTime(1);
            Assert.AreEqual("warrior", s.Actor.Definition.Id);
            Assert.IsTrue(s.TryCommit(CombatAction.Ether, mage, out _)); Assert.IsTrue(s.ApplyImpact());
            Assert.AreEqual(18, s.Pending.EffectiveAmount);
            yield return Capture("effective-mana-and-log", 1280, 720);
            Assert.IsTrue(Object.FindObjectsByType<Text>(FindObjectsSortMode.None).Any(t => t.text == "+18 MANÁ"));
            Assert.AreEqual(4, s.CombatLog.Count);
            Battle.RestartCombat(); Assert.AreEqual(0, hud.VisibleNumbers); s = Battle.Session;
            mage = s.Fighters.Single(f => f.Definition.Id == "mage"); mage.Damage(mage.Health - 3);
            mage.ApplyStatus(new EffectDefinition(Resources.Load<EffectData>("Encounters/poison")));
            s.AdvanceTime(4); yield return null;
            Assert.IsFalse(mage.IsAlive); Assert.IsNull(s.Actor); Assert.AreEqual(1, hud.VisibleNumbers);
            Assert.IsTrue(Object.FindObjectsByType<Text>(FindObjectsSortMode.None).Any(t => t.text == "−3 VIDA"));
            yield return Capture("lethal-poison-effective", 1280, 720);
            Battle.RestartCombat(); yield return null; Assert.AreEqual(0, hud.VisibleNumbers);
        }
        [UnityTest] public IEnumerator HeldConfirmDoesNotCloseHelpAndDeviceSwitchKeepsFocus()
        {
            yield return Press(false, Key.DownArrow);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter)); yield return null; yield return null;
            Assert.IsTrue(Main.ControlsOpen); yield return new WaitForSecondsRealtime(.5f); Assert.IsTrue(Main.ControlsOpen);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
            yield return Pad(GamepadButton.DpadRight); Assert.AreEqual(1, Main.HelpPage); Assert.IsTrue(SessionOptions.UsingGamepad); AssertSelection("ControlsBackButton");
            yield return Press(false, Key.LeftArrow); Assert.AreEqual(0, Main.HelpPage); Assert.IsFalse(SessionOptions.UsingGamepad); AssertSelection("ControlsBackButton");
        }
        private IEnumerator Press(bool pad, Key key)
        {
            if (pad)
            {
                var button = key == Key.UpArrow ? GamepadButton.DpadUp : key == Key.DownArrow ? GamepadButton.DpadDown : key == Key.LeftArrow ? GamepadButton.DpadLeft : key == Key.RightArrow ? GamepadButton.DpadRight : key == Key.Enter ? GamepadButton.South : GamepadButton.East;
                yield return Pad(button); yield break;
            }
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
        }
        private IEnumerator Pad(GamepadButton button)
        {
            InputSystem.QueueStateEvent(gamepad, new GamepadState().WithButton(button)); yield return null; yield return null;
            InputSystem.QueueStateEvent(gamepad, new GamepadState()); yield return null; yield return null;
        }
        private static void AssertSelection(string name) => Assert.AreEqual(name, EventSystem.current.currentSelectedGameObject?.name);
        private static IEnumerator WaitScene(string name)
        {
            float deadline = Time.realtimeSinceStartup + 10;
            while (SceneManager.GetActiveScene().name != name && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreEqual(name, SceneManager.GetActiveScene().name); yield return null; yield return null;
        }
        private IEnumerator Capture(string name, int width, int height)
        {
            yield return null;
            var camera = Camera.main; var canvas = Object.FindFirstObjectByType<Canvas>();
            var mode = canvas.renderMode; var world = canvas.worldCamera; var distance = canvas.planeDistance;
            var target = camera.targetTexture; var active = RenderTexture.active; float aspect = camera.aspect;
            var rt = RenderTexture.GetTemporary(width, height, 24); Texture2D image = null;
            try
            {
                camera.targetTexture = rt; camera.aspect = (float)width / height;
                canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
                yield return null; Canvas.ForceUpdateCanvases();
                foreach (var t in canvas.GetComponentsInChildren<Text>().Where(t => t.enabled && t.gameObject.activeInHierarchy && !string.IsNullOrEmpty(t.text)))
                    Assert.LessOrEqual(t.preferredHeight, t.rectTransform.rect.height + 2, "Text cropped: " + t.name + " / " + t.text);
                foreach (var icon in canvas.GetComponentsInChildren<Image>().Where(i => i.name.StartsWith("Icon_"))) Assert.IsNotNull(icon.sprite, "Missing icon: " + icon.name);
                camera.Render(); RenderTexture.active = rt; image = new Texture2D(width, height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
                string directory = InterfaceEvidence.Root;
                File.WriteAllBytes(Path.Combine(directory, name + "-" + width + "x" + height + ".png"), image.EncodeToPNG());
            }
            finally
            {
                canvas.renderMode = mode; canvas.worldCamera = world; canvas.planeDistance = distance;
                camera.targetTexture = target; camera.aspect = aspect; RenderTexture.active = active;
                if (image != null) Object.Destroy(image); RenderTexture.ReleaseTemporary(rt);
            }
        }
    }
}
