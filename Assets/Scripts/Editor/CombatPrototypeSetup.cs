using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

namespace UltimaGuardia.Editor
{
    public static class CombatPrototypeSetup
    {
        // Explicit authoring utility, never invoked by import/startup. Only builds Combat.
        [MenuItem("Ultima Guardia/H01/Crear arena y combate provisional")]
        public static void BuildScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Detener Play y guardar la escena antes de preparar H01.");
            Directory.CreateDirectory("Assets/Materials/H01");
            Directory.CreateDirectory("Assets/Prefabs/H01");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0, 7.8f, -14);
            camera.transform.LookAt(new Vector3(0, -0.5f, 2));
            camera.fieldOfView = 44;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.045f, 0.065f, 0.095f);
            camera.nearClipPlane = 0.1f;
            var light = new GameObject("Directional Light", typeof(Light)).GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.5f;
            light.transform.rotation = Quaternion.Euler(45, -35, 0);
            light.shadows = LightShadows.Soft;
            RenderSettings.ambientLight = new Color(0.45f, 0.50f, 0.60f);

            var stone = Material("Stone", new Color(0.25f, 0.30f, 0.37f));
            var floor = Material("Floor", new Color(0.13f, 0.19f, 0.24f));
            var gate = Material("Gate", new Color(0.24f, 0.16f, 0.10f));
            var arena = new GameObject("Arena_Blockout_Provisional").transform;
            Box(arena, "Suelo", new Vector3(0, -0.22f, 2), new Vector3(18, 0.4f, 12), floor);
            Box(arena, "Muro izquierdo", new Vector3(-5.1f, 2, 6.5f), new Vector3(6, 4, 0.8f), stone);
            Box(arena, "Muro derecho", new Vector3(5.1f, 2, 6.5f), new Vector3(6, 4, 0.8f), stone);
            Box(arena, "Dintel", new Vector3(0, 3.7f, 6.5f), new Vector3(4.2f, 0.6f, 0.8f), stone);
            Box(arena, "Portón", new Vector3(0, 1.6f, 6.7f), new Vector3(4.2f, 3.2f, 0.35f), gate);
            Box(arena, "Marco izquierdo", new Vector3(-2.2f, 1.7f, 6.1f), new Vector3(0.45f, 3.4f, 1), stone);
            Box(arena, "Marco derecho", new Vector3(2.2f, 1.7f, 6.1f), new Vector3(0.45f, 3.4f, 1), stone);

            var anchors = new GameObject("CombatAnchors").transform;
            var views = new[]
            {
                Fighter(anchors, "warrior", "Guerrero", PrimitiveType.Capsule, new Vector3(-2, 0, 0.7f), new Vector3(1, 1, 1), new Color(0.18f, 0.48f, 0.72f)),
                Fighter(anchors, "mage", "Maga", PrimitiveType.Cylinder, new Vector3(-4, 0, 2.5f), new Vector3(0.8f, 0.85f, 0.8f), new Color(0.52f, 0.32f, 0.78f)),
                Fighter(anchors, "ogre", "Ogro", PrimitiveType.Cube, new Vector3(2, 0, 0.9f), new Vector3(1.6f, 2.6f, 1.3f), new Color(0.68f, 0.33f, 0.19f)),
                Fighter(anchors, "shaman", "Chamán", PrimitiveType.Sphere, new Vector3(4, 0, 2.7f), new Vector3(1.2f, 1.6f, 1.2f), new Color(0.30f, 0.58f, 0.34f))
            };

            var canvasObject = new GameObject("CombatCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;
            var root = canvas.transform;
            Panel(root, "Header", 0, 290, 1280, 140, new Color(0.035f, 0.055f, 0.085f, 0.96f));
            var turn = Label(root, "Turn", "", 24, 0, 327, 1220, 42);
            var labels = new Text[4];
            for (int i = 0; i < 4; i++)
                labels[i] = Label(root, "Resources_" + views[i].combatantId, "", 20, -465 + i * 310, 265, 300, 80);
            Panel(root, "EventBackground", 0, -82, 1240, 54, new Color(0.035f, 0.055f, 0.085f, 0.95f));
            var events = Label(root, "LastEvent", "", 21, 0, -82, 1200, 48);
            var decision = Panel(root, "Decision", 0, -201, 1240, 180, new Color(0.045f, 0.075f, 0.115f, 0.98f));
            var commands = Label(decision.transform, "Commands", "", 25, -375, 0, 470, 170);
            var description = Label(decision.transform, "Description", "", 22, 245, 0, 710, 160);
            var help = Label(root, "Help", "", 18, 0, -325, 1240, 62);
            var overlay = Panel(root, "Overlay", 0, 0, 1280, 720, new Color(0.02f, 0.035f, 0.06f, 0.96f));
            var title = Label(overlay.transform, "OverlayTitle", "", 48, 0, 160, 1000, 100);
            var options = Label(overlay.transform, "OverlayOptions", "", 32, 0, -20, 800, 250);
            Label(overlay.transform, "OverlayHelp", "↑↓: elegir · Enter / botón inferior: confirmar", 22, 0, -245, 1100, 60);
            overlay.SetActive(false);
            var controller = new GameObject("CombatController").AddComponent<CombatPrototype>();
            controller.Configure(AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Data/Input/GuardiaInput.inputactions"),
                views, labels, turn, events, commands, description, help, decision, overlay, title, options, camera);
            PlayerSettings.bundleVersion = "0.0.2";
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Combat.unity");
            Debug.Log("H01: Combat integrada. MainMenu y configuración gráfica conservados.");
        }

        private static Material Material(string name, Color color)
        {
            string path = "Assets/Materials/H01/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void Box(Transform parent, string name, Vector3 position, Vector3 size, Material material)
        {
            var mesh = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            mesh.name = name;
            mesh.transform.SetParent(parent, false);
            mesh.transform.position = position;
            mesh.GetComponent<MeshRenderer>().sharedMaterial = material;
            mesh.ToMesh(); mesh.Refresh();
        }

        private static CombatantView Fighter(Transform parent, string id, string name, PrimitiveType primitive,
            Vector3 position, Vector3 scale, Color color)
        {
            var root = new GameObject(name + "_Placeholder");
            var view = root.AddComponent<CombatantView>();
            view.combatantId = id;
            var body = GameObject.CreatePrimitive(primitive);
            body.name = "Visual_Provisional";
            body.transform.SetParent(root.transform, false);
            body.transform.localScale = scale;
            float height = (primitive == PrimitiveType.Capsule || primitive == PrimitiveType.Cylinder) ? scale.y * 2 : scale.y;
            body.transform.localPosition = new Vector3(0, height / 2, 0);
            body.GetComponent<Renderer>().sharedMaterial = Material(id, color);
            view.visual = body.transform;
            view.projectileOrigin = Socket(root.transform, "ProjectileOrigin", new Vector3(0, height * 0.7f, 0.7f));
            view.effectAnchor = Socket(root.transform, "EffectAnchor", new Vector3(0, height * 0.6f, 0));
            view.actorMarker = Marker(root.transform, "ActorMarker", new Color(1, 0.8f, 0.2f), 1.5f, 0.02f);
            view.targetMarker = Marker(root.transform, "TargetMarker", Color.white, 1.8f, 0.05f);
            var label = new GameObject("Name", typeof(TextMesh));
            label.transform.SetParent(root.transform, false);
            label.transform.localPosition = new Vector3(0, height + 0.45f, 0);
            view.nameLabel = label.GetComponent<TextMesh>();
            view.nameLabel.text = name;
            view.nameLabel.fontSize = 40;
            view.nameLabel.characterSize = 0.085f;
            view.nameLabel.anchor = TextAnchor.MiddleCenter;
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/H01/" + id + ".prefab");
            UnityEngine.Object.DestroyImmediate(root);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            var anchor = new GameObject("Anchor_" + id).transform;
            anchor.SetParent(parent, false);
            anchor.position = position;
            anchor.rotation = Quaternion.Euler(0, position.x < 0 ? 90 : -90, 0);
            instance.transform.SetParent(anchor, false);
            return instance.GetComponent<CombatantView>();
        }

        private static Transform Socket(Transform parent, string name, Vector3 position)
        {
            var socket = new GameObject(name).transform;
            socket.SetParent(parent, false); socket.localPosition = position;
            return socket;
        }

        private static GameObject Marker(Transform parent, string name, Color color, float width, float y)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = name;
            marker.transform.SetParent(parent, false);
            marker.transform.localPosition = new Vector3(0, y, 0);
            marker.transform.localScale = new Vector3(width, 0.015f, width);
            UnityEngine.Object.DestroyImmediate(marker.GetComponent<Collider>());
            marker.GetComponent<Renderer>().sharedMaterial = Material(name, color);
            marker.SetActive(false);
            return marker;
        }

        private static GameObject Panel(Transform parent, string name, float x, float y, float width, float height, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height); rect.anchoredPosition = new Vector2(x, y);
            go.GetComponent<Image>().color = color;
            return go;
        }

        private static Text Label(Transform parent, string name, string value, int size, float x, float y, float width, float height)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height); rect.anchoredPosition = new Vector2(x, y);
            var label = go.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = value; label.fontSize = size; label.alignment = TextAnchor.MiddleLeft;
            label.color = new Color(0.94f, 0.96f, 1);
            label.raycastTarget = false;
            return label;
        }
    }
}
