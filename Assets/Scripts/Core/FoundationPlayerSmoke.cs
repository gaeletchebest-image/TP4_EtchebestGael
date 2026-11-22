#if DEVELOPMENT_BUILD && !UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

namespace UltimaGuardia
{
    // Opt-in validation of the real development player. Absent from release builds.
    public sealed class FoundationPlayerSmoke : MonoBehaviour
    {
        [Serializable]
        private sealed class Result
        {
            public string version = "0.0.7-H06";
            public string unity;
            public string initialScene;
            public int width;
            public int height;
            public int nativeWindowWidth;
            public int nativeWindowHeight;
            public bool passed;
            public bool exitedThroughMenu;
            public bool animatedActionVerified;
            public bool audioVerified;
            public string error;
            public string devices = "Virtual Keyboard and Gamepad";
            public string captureMethod = "Offscreen Camera.Render; temporary ScreenSpaceCamera canvas in hidden validation process";
        }

        private Keyboard keyboard;
        private Gamepad gamepad;
        private Result result = new Result();
        private string directory;
        private bool expectingQuit;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Begin()
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-foundation-smoke") < 0) return;
            var host = new GameObject("FoundationPlayerSmoke");
            DontDestroyOnLoad(host);
            host.AddComponent<FoundationPlayerSmoke>();
        }

        private IEnumerator Start()
        {
            Application.runInBackground = true;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            directory = Path.GetFullPath(Path.Combine(Application.dataPath, "../Validation"));
            Directory.CreateDirectory(directory);
            result.unity = Application.unityVersion;
            result.version = Application.version + "-flow";
            result.initialScene = SceneManager.GetActiveScene().name;
            keyboard = InputSystem.AddDevice<Keyboard>();
            gamepad = InputSystem.AddDevice<Gamepad>();
            yield return new WaitForSecondsRealtime(1);
            result.nativeWindowWidth = Screen.width;
            result.nativeWindowHeight = Screen.height;
            result.width = RequestedDimension("-screen-width", Screen.width);
            result.height = RequestedDimension("-screen-height", Screen.height);
            Check(result.initialScene == "MainMenu", "Initial scene must be MainMenu");
            CheckSelection("PlayButton");
            yield return Capture("main-menu");

            yield return KeyPress(Key.DownArrow);
            CheckSelection("ControlsButton");
            yield return KeyPress(Key.Enter);
            Check(Menu().ControlsOpen, "Keyboard confirm must open controls");
            yield return Capture("controls");
            yield return KeyPress(Key.Escape);
            Check(!Menu().ControlsOpen, "Keyboard cancel must close controls");

            yield return PadPress(GamepadButton.DpadUp);
            CheckSelection("PlayButton");
            yield return PadPress(GamepadButton.South);
            yield return WaitForScene("Combat");
            var audioSources = GameAudio.Current.GetComponents<AudioSource>();
            Check(audioSources.Length >= 3 && audioSources[0].clip != null && audioSources[0].clip.name == "battle_loop" && audioSources[0].isPlaying,
                "Battle music must start in the player");
            yield return Capture("combat");
            float deadline = Time.realtimeSinceStartup + 10;
            while (Battle().Phase != PrototypePhase.Commands && Time.realtimeSinceStartup < deadline) yield return null;
            Check(Battle().Session.Actor?.Definition.Id == "mage", "Mage must receive the first turn");
            yield return KeyPress(Key.Enter); yield return KeyPress(Key.Enter);
            Check(Battle().Phase == PrototypePhase.Executing, "Basic must start animation");
            yield return KeyPress(Key.Escape);
            Check(Battle().IsPaused, "Pause during animated action");
            Check(!audioSources[0].isPlaying, "Battle music must pause");
            float actionTime = Battle().ActionTime;
            var animation = FindFirstObjectByType<CharacterAnimationPlayer>();
            float animationTime = animation.Elapsed;
            yield return new WaitForSecondsRealtime(.35f);
            Check(actionTime == Battle().ActionTime && animationTime == animation.Elapsed, "Animation clock must freeze");
            yield return Capture("animated-action-paused");
            yield return KeyPress(Key.Escape);
            Check(audioSources[0].isPlaying, "Battle music must resume");
            deadline = Time.realtimeSinceStartup + 10;
            while (Battle().Session.CompletedActions < 1 && Time.realtimeSinceStartup < deadline) yield return null;
            Check(Battle().Session.CompletedActions >= 1, "Animated action must finish");
            bool enemyDamaged = false;
            foreach (var fighter in Battle().Session.Fighters)
                if (fighter.Definition.Team == CombatTeam.Enemies && fighter.Health < fighter.Definition.MaxHealth) enemyDamaged = true;
            Check(enemyDamaged, "Animated projectile must apply damage");
            result.animatedActionVerified = true;
            result.audioVerified = true;
            yield return Capture("animated-action-complete");
            yield return PadPress(GamepadButton.Start);
            Check(Battle().IsPaused, "Gamepad menu must pause");
            yield return Capture("pause");
            yield return PadPress(GamepadButton.East);
            Check(!Battle().IsPaused, "Gamepad cancel must resume");
            yield return KeyPress(Key.Escape);
            Check(Battle().IsPaused, "Keyboard Escape must pause");
            yield return KeyPress(Key.Enter);
            Check(!Battle().IsPaused, "Keyboard confirm must resume");
            yield return KeyPress(Key.Escape);
            for (int i = 0; i < 4; i++) yield return KeyPress(Key.DownArrow);
            yield return KeyPress(Key.Enter);
            Check(Battle().PausePage == 3, "Return requires confirmation");
            yield return KeyPress(Key.DownArrow); yield return KeyPress(Key.Enter);
            yield return WaitForScene("MainMenu");
            for (int i = 0; i < 3; i++) yield return KeyPress(Key.DownArrow);
            CheckSelection("QuitButton");
            expectingQuit = true;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return new WaitForSecondsRealtime(3);
            Fail("Salir did not close the player");
        }

        private IEnumerator Capture(string name)
        {
            string file = Path.Combine(directory, name + "-" + result.width + "x" + result.height + ".png");
            // A hidden Windows player does not present its backbuffer. Render the same
            // scene UI explicitly, without requiring Windows UI automation or focus.
            var camera = Camera.main;
            var canvas = FindFirstObjectByType<Canvas>();
            var previousMode = canvas.renderMode;
            var previousCamera = canvas.worldCamera;
            var previousTexture = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var target = RenderTexture.GetTemporary(result.width, result.height, 24, RenderTextureFormat.ARGB32);
            Texture2D image = null;
            try
            {
                camera.targetTexture = target;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 1;
                yield return null;
                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = target;
                image = new Texture2D(result.width, result.height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, result.width, result.height), 0, 0);
                image.Apply();
                int readablePixels = 0;
                foreach (var pixel in image.GetPixels32())
                    if (pixel.r > 180 && pixel.g > 150 && pixel.b > 85) readablePixels++;
                Check(readablePixels > 100, "Capture has no visible foreground: " + name);
                File.WriteAllBytes(file, image.EncodeToPNG());
            }
            finally
            {
                canvas.renderMode = previousMode;
                canvas.worldCamera = previousCamera;
                camera.targetTexture = previousTexture;
                RenderTexture.active = previousActive;
                if (image != null) Destroy(image);
                RenderTexture.ReleaseTemporary(target);
            }
            Check(File.Exists(file), "Screenshot missing: " + name);
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

        private IEnumerator WaitForScene(string name)
        {
            float deadline = Time.realtimeSinceStartup + 10;
            while (SceneManager.GetActiveScene().name != name && Time.realtimeSinceStartup < deadline)
                yield return null;
            Check(SceneManager.GetActiveScene().name == name, "Scene not loaded: " + name);
            yield return null;
            yield return null;
        }

        private static FoundationMenu Menu() => FindFirstObjectByType<FoundationMenu>();
        private static CombatPrototype Battle() => FindFirstObjectByType<CombatPrototype>();

        private static int RequestedDimension(string argument, int fallback)
        {
            var arguments = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(arguments, argument);
            return index >= 0 && index + 1 < arguments.Length && int.TryParse(arguments[index + 1], out int value)
                && value > 0 && value <= 4096 ? value : fallback;
        }

        private void CheckSelection(string name)
        {
            Check(EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null &&
                EventSystem.current.currentSelectedGameObject.name == name, "Incorrect selection: " + name);
        }

        private void Check(bool condition, string message)
        {
            if (!condition) Fail(message);
        }

        private void Fail(string message)
        {
            result.error = message;
            Save();
            Debug.LogError("H00 smoke failed: " + message);
            Application.Quit(1);
            throw new InvalidOperationException(message);
        }

        private void OnApplicationQuit()
        {
            result.passed = expectingQuit && string.IsNullOrEmpty(result.error);
            result.exitedThroughMenu = result.passed;
            Save();
        }

        private void Save()
        {
            if (directory != null)
                File.WriteAllText(Path.Combine(directory, "result-" + result.width + "x" + result.height + ".json"),
                    JsonUtility.ToJson(result, true));
        }
    }
}
#endif
