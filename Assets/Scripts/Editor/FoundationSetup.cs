using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace UltimaGuardia.Editor
{
    public static class FoundationSetup
    {
        // Explicitly invoked only; never rewrites scenes on import or startup.
        [MenuItem("Ultima Guardia/H00/Regenerar escenas base")]
        public static void CreateProject()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.InvalidOperationException("Detener Play antes de preparar las escenas.");
            var current = EditorSceneManager.GetActiveScene();
            if (current.isDirty)
                throw new System.InvalidOperationException("Guardar los cambios de la escena abierta antes de continuar.");

            EditorSettings.serializationMode = SerializationMode.ForceText;
            PlayerSettings.productName = "Última Guardia";
            PlayerSettings.companyName = "UltimaGuardia";
            PlayerSettings.bundleVersion = "0.0.1";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = false;
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            QualitySettings.SetQualityLevel(1, true); // Preserve the existing PC URP asset.
            var input = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Data/Input/GuardiaInput.inputactions");
            if (input == null) throw new System.InvalidOperationException("Falta importar GuardiaInput.");
            CreateScene("MainMenu", false, input);
            CreateScene("Combat", true, input);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Combat.unity", true)
            };
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Debug.Log("H00: escenas y configuración base preparadas.");
        }

        private static void CreateScene(string name, bool combat, InputActionAsset input)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0, 2, -10);
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.055f, 0.085f);
            new GameObject("Directional Light", typeof(Light)).GetComponent<Light>().type = LightType.Directional;

            var eventObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            var module = eventObject.GetComponent<InputSystemUIInputModule>();
            module.UnassignActions();
            var canvasObject = new GameObject("FoundationCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            var controller = canvasObject.AddComponent<FoundationMenu>();

            var main = Panel("Main", canvasObject.transform);
            Label(main.transform, "Title", combat ? "Defendé la entrada" : "ÚLTIMA GUARDIA", 48, 205);
            Label(main.transform, "Subtitle", combat ? "Derrotá al ogro y al chamán." : "Guerrero y maga · Una última defensa", 24, 140);
            if (combat) Label(main.transform, "Scaffold", "Escena preparada", 22, 55);

            var first = Button(main.transform, combat ? "BackButton" : "PlayButton",
                combat ? "Volver al menú" : "Jugar", combat ? -35 : 30,
                combat ? (UnityAction)controller.ReturnToMenu : controller.Play);
            if (!combat)
            {
                Button(main.transform, "ControlsButton", "Controles", -45, controller.OpenControls);
                Button(main.transform, "QuitButton", "Salir", -120, controller.Quit);
            }
            Label(main.transform, "NavigationHelp", "Flechas / cruceta / stick: navegar     Enter / Espacio / botón inferior: confirmar", 20, -245);
            Label(main.transform, "BackHelp", "Escape / botón derecho: volver     Menú del control: pausa", 20, -285);

            var controls = Panel("Controls", canvasObject.transform);
            Label(controls.transform, "Title", "Controles", 42, 185);
            Label(controls.transform, "Instructions", "Navegar: flechas, cruceta o stick izquierdo\nConfirmar: Enter, Espacio o botón inferior\nVolver: Escape o botón derecho\nPausa: Escape o botón de menú del control", 26, 40, 900, 190);
            var controlsBack = Button(controls.transform, "ControlsBackButton", "Volver", -140, controller.CloseControls);
            controls.SetActive(false);

            var pause = Panel("Pause", canvasObject.transform);
            Label(pause.transform, "Title", "Pausa", 42, 155);
            var resume = Button(pause.transform, "ContinueButton", "Continuar", 20, controller.TogglePause);
            Button(pause.transform, "PauseBackButton", "Volver al menú", -55, controller.ReturnToMenu);
            pause.SetActive(false);

            controller.Configure(input, module, main, controls, pause, first.gameObject,
                controlsBack.gameObject, resume.gameObject, combat);
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/" + name + ".unity");
        }

        private static GameObject Panel(string name, Transform parent)
        {
            var panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            return panel;
        }

        private static Text Label(Transform parent, string name, string text, int size,
            float y, float width = 1120, float height = 80)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(0, y);
            var label = go.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = text;
            label.fontSize = size;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.92f, 0.94f, 0.97f);
            label.raycastTarget = false;
            return label;
        }

        private static Button Button(Transform parent, string name, string text, float y, UnityAction action)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 58);
            rect.anchoredPosition = new Vector2(0, y);
            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = new Color(0.10f, 0.16f, 0.23f);
            colors.highlightedColor = colors.selectedColor = new Color(0.20f, 0.39f, 0.53f);
            colors.pressedColor = new Color(0.29f, 0.49f, 0.60f);
            button.colors = colors;
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.Vertical;
            button.navigation = navigation;
            UnityEventTools.AddPersistentListener(button.onClick, action);
            Label(go.transform, "Label", text, 26, 0, 390, 56);
            return button;
        }
    }
}
