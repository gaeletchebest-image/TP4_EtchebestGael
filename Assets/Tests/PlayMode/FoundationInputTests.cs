using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UltimaGuardia.Tests
{
    // Integration tests drive real bindings and the EventSystem, rather than invoking UI callbacks.
    public class FoundationInputTests
    {
#if UNITY_EDITOR
        private InputSettings.EditorInputBehaviorInPlayMode previousEditorInput;
#endif
        private Keyboard keyboard;
        private Gamepad gamepad;
        private bool previousBackground;
        private InputSettings.BackgroundBehavior previousBehavior;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            previousBackground = Application.runInBackground;
            previousBehavior = InputSystem.settings.backgroundBehavior;
            Application.runInBackground = true;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
            previousEditorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            keyboard = InputSystem.AddDevice<Keyboard>();
            gamepad = InputSystem.AddDevice<Gamepad>();
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return null;
            yield return null;
            AssertSelection("PlayButton");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (keyboard != null && keyboard.added) InputSystem.RemoveDevice(keyboard);
            if (gamepad != null && gamepad.added) InputSystem.RemoveDevice(gamepad);
#if UNITY_EDITOR
            InputSystem.settings.editorInputBehaviorInPlayMode = previousEditorInput;
#endif
            InputSystem.settings.backgroundBehavior = previousBehavior;
            Application.runInBackground = previousBackground;
            yield return null;
        }

        [UnityTest]
        public IEnumerator KeyboardNavigatesConfirmsCancelsPausesAndReturns()
        {
            yield return KeyPress(Key.DownArrow);
            AssertSelection("ControlsButton");
            yield return KeyPress(Key.Space);
            Assert.IsTrue(Menu().ControlsOpen);
            AssertSelection("ControlsBackButton");
            yield return KeyPress(Key.Escape);
            Assert.IsFalse(Menu().ControlsOpen);
            AssertSelection("ControlsButton");
            yield return KeyPress(Key.UpArrow);
            yield return KeyPress(Key.Enter);
            yield return WaitForScene("Combat");
            Assert.AreEqual(PrototypePhase.Advancing, Battle().Phase);
            yield return KeyPress(Key.Escape);
            Assert.IsTrue(Battle().IsPaused);
            yield return KeyPress(Key.Escape);
            Assert.IsFalse(Battle().IsPaused);
            yield return KeyPress(Key.Escape);
            for (int i = 0; i < 4; i++) yield return KeyPress(Key.DownArrow);
            yield return KeyPress(Key.Enter);
            Assert.AreEqual(3, Battle().PausePage);
            yield return KeyPress(Key.DownArrow); yield return KeyPress(Key.Enter);
            yield return WaitForScene("MainMenu");
            AssertSelection("PlayButton");
        }

        [UnityTest]
        public IEnumerator VirtualGamepadUsesDpadStickFaceButtonsAndMenu()
        {
            yield return PadPress(GamepadButton.DpadDown);
            AssertSelection("ControlsButton");
            yield return PadPress(GamepadButton.South);
            Assert.IsTrue(Menu().ControlsOpen);
            yield return PadPress(GamepadButton.East);
            Assert.IsFalse(Menu().ControlsOpen);
            AssertSelection("ControlsButton");
            InputSystem.QueueStateEvent(gamepad, new GamepadState { leftStick = Vector2.up });
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(gamepad, new GamepadState());
            yield return null;
            AssertSelection("PlayButton");
            yield return PadPress(GamepadButton.South);
            yield return WaitForScene("Combat");
            yield return PadPress(GamepadButton.Start);
            Assert.IsTrue(Battle().IsPaused);
            yield return PadPress(GamepadButton.East);
            Assert.IsFalse(Battle().IsPaused);
            yield return PadPress(GamepadButton.Start);
            for (int i = 0; i < 4; i++) yield return PadPress(GamepadButton.DpadDown);
            yield return PadPress(GamepadButton.South);
            Assert.AreEqual(3, Battle().PausePage);
            yield return PadPress(GamepadButton.DpadDown); yield return PadPress(GamepadButton.South);
            yield return WaitForScene("MainMenu");
        }

        [UnityTest]
        public IEnumerator HeldConfirmDoesNotConfirmTheNextScene()
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return WaitForScene("Combat");
            yield return new WaitForSecondsRealtime(0.5f);
            Assert.AreEqual("Combat", SceneManager.GetActiveScene().name);
            Assert.AreEqual(PrototypePhase.Advancing, Battle().Phase);
            Assert.IsNull(Battle().Session.Pending);
            yield return new WaitForSecondsRealtime(4);
            Assert.AreEqual(PrototypePhase.Commands, Battle().Phase);
            Assert.IsNull(Battle().Session.Pending, "Held confirm must not select or commit when ATB becomes ready.");
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
        }

        private IEnumerator KeyPress(Key key)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            yield return null;
        }

        private IEnumerator PadPress(GamepadButton button)
        {
            InputSystem.QueueStateEvent(gamepad, new GamepadState().WithButton(button));
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(gamepad, new GamepadState());
            yield return null;
            yield return null;
        }

        private static FoundationMenu Menu() => Object.FindFirstObjectByType<FoundationMenu>();
        private static CombatPrototype Battle() => Object.FindFirstObjectByType<CombatPrototype>();

        private static void AssertSelection(string name)
        {
            Assert.IsNotNull(EventSystem.current);
            Assert.IsNotNull(EventSystem.current.currentSelectedGameObject);
            Assert.AreEqual(name, EventSystem.current.currentSelectedGameObject.name);
        }

        private static IEnumerator WaitForScene(string name)
        {
            float deadline = Time.realtimeSinceStartup + 10;
            while (SceneManager.GetActiveScene().name != name && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.AreEqual(name, SceneManager.GetActiveScene().name);
            yield return null;
            yield return null;
        }
    }
}
