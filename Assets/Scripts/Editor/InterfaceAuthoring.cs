using System;
using System.IO;
using System.Linq;
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
    public static class InterfaceAuthoring
    {
        public static readonly string[] IconNames = { "bar", "warrior", "mage", "ogre", "shaman", "life", "mana", "atb", "recovery", "sword", "heavy", "magic", "heal", "storm", "poison", "haste", "corrosion", "potion", "ether", "tonic", "flask", "antidote", "target", "keyboard", "gamepad", "fortress" };
        [MenuItem("Ultima Guardia/H08/Construir interfaz y menús finales")]
        public static void Integrate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Detener Play y guardar la escena antes de integrar H08.");
            GenerateIcons();
            EditorSceneManager.OpenScene("Assets/Scenes/Combat.unity");
            var battle = UnityEngine.Object.FindFirstObjectByType<CombatPrototype>();
            var input = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Data/Input/GuardiaInput.inputactions");
            var views = UnityEngine.Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None).OrderBy(v => Array.IndexOf(new[] { "warrior", "mage", "ogre", "shaman" }, v.combatantId)).ToArray();
            var encounter = battle.Encounter;
            foreach (var old in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None)) UnityEngine.Object.DestroyImmediate(old.gameObject);
            var canvas = Canvas("CombatCanvas");
            var hud = canvas.gameObject.AddComponent<FinalCombatHud>();
            hud.Build(canvas.transform, out var labels, out var bars, out var turn, out var events, out var commands, out var description,
                out var help, out var decision, out var overlay, out var title, out var options);
            battle.Configure(input, views, labels, turn, events, commands, description, help, decision, overlay, title, options, Camera.main);
            battle.ConfigureEncounter(encounter, bars); battle.ConfigureFinal(hud);
            foreach (var slot in encounter.participants)
            {
                slot.combatant.portrait = Resources.Load<Sprite>("Interface/" + slot.combatant.id); EditorUtility.SetDirty(slot.combatant);
                foreach (var action in slot.combatant.actions)
                {
                    string icon = action.command == CombatAction.Basic ? (slot.combatant.id == "warrior" || slot.combatant.id == "ogre" ? "sword" : "magic") : FinalCombatHud.ActionIcon(action.command);
                    action.icon = Resources.Load<Sprite>("Interface/" + icon); EditorUtility.SetDirty(action);
                }
            }
            foreach (var slot in encounter.inventory)
            {
                slot.item.icon = Resources.Load<Sprite>("Interface/" + FinalCombatHud.ActionIcon(slot.item.command));
                EditorUtility.SetDirty(slot.item);
            }
            foreach (var guid in AssetDatabase.FindAssets("t:EffectData", new[] { "Assets/Data" }))
            {
                var effect = AssetDatabase.LoadAssetAtPath<EffectData>(AssetDatabase.GUIDToAssetPath(guid));
                effect.icon = Resources.Load<Sprite>("Interface/" + new[] { "poison", "haste", "corrosion" }[(int)effect.kind]); EditorUtility.SetDirty(effect);
            }
            foreach (var view in views)
            {
                ReplaceMarker(view.actorMarker, true); ReplaceMarker(view.targetMarker, false);
                PrefabUtility.RecordPrefabInstancePropertyModifications(view.actorMarker.GetComponent<MeshFilter>());
                PrefabUtility.RecordPrefabInstancePropertyModifications(view.targetMarker.GetComponent<MeshFilter>());
            }
            EditorUtility.SetDirty(battle); EditorUtility.SetDirty(hud);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            BuildMain(input);
            PlayerSettings.bundleVersion = "0.0.9";
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Debug.Log("H08: interfaz, recursos y menús integrados; reglas, anclajes, personajes y fortaleza conservados.");
        }

        private static Canvas Canvas(string name)
        {
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .5f; return canvas;
        }
        private static Button Button(Transform parent, string name, string label, float x, float y, float width, UnityAction action)
        {
            var image = InterfaceTheme.Box(parent, name, x, y, width, 52, Color.white); image.raycastTarget = true;
            var button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image;
            var colors = button.colors; colors.normalColor = InterfaceTheme.Panel; colors.highlightedColor = colors.selectedColor = new Color32(68, 78, 83, 255);
            colors.pressedColor = new Color32(103, 98, 78, 255); colors.fadeDuration = .08f; button.colors = colors;
            var navigation = button.navigation; navigation.mode = Navigation.Mode.None; button.navigation = navigation;
            InterfaceTheme.Box(image.transform, "Accent", -width / 2 + 1, 0, 2, 52, InterfaceTheme.Gold);
            InterfaceTheme.MenuText(image.transform, "Label", label, 24, 12, 0, width - 36, 48).color = InterfaceTheme.Gold;
            UnityEventTools.AddPersistentListener(button.onClick, action); return button;
        }
        private static void BuildMain(InputActionAsset input)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            foreach (var old in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None)) UnityEngine.Object.DestroyImmediate(old.gameObject);
            var module = UnityEngine.Object.FindFirstObjectByType<InputSystemUIInputModule>();
            var canvas = Canvas("FoundationCanvas"); var controller = canvas.gameObject.AddComponent<FoundationMenu>();
            InterfaceTheme.Box(canvas.transform, "Background", 0, 0, 1280, 720, InterfaceTheme.Ink);
            InterfaceTheme.Box(canvas.transform, "LeftRule", -580, 0, 2, 580, InterfaceTheme.Gold);
            var main = InterfaceTheme.Rect(canvas.transform, "Main", 0, 0, 1280, 720).gameObject;
            InterfaceTheme.MenuText(main.transform, "Eyebrow", "UNA FORTALEZA. CUATRO DESTINOS.", 16, -267, 250, 600, 36).color = InterfaceTheme.Muted;
            InterfaceTheme.MenuText(main.transform, "Title", "ÚLTIMA\nGUARDIA", 64, -266, 142, 600, 180).color = InterfaceTheme.Gold;
            InterfaceTheme.MenuText(main.transform, "Subtitle", "Defendé la entrada. Elegí tu próxima acción.", 20, -266, 28, 600, 50).color = InterfaceTheme.Muted;
            InterfaceTheme.Icon(main.transform, "fortress", 320, 34, 380, new Color32(124, 143, 150, 255));
            InterfaceTheme.MenuText(main.transform, "Encounter", "GUERRERO + MAGA\nCONTRA OGRO + CHAMÁN", 18, 320, -211, 400, 80, TextAnchor.MiddleCenter).color = InterfaceTheme.Muted;
            var buttons = new[]
            {
                Button(main.transform, "PlayButton", "Jugar", -330, -56, 460, controller.Play),
                Button(main.transform, "ControlsButton", "Cómo jugar", -330, -118, 460, controller.OpenControls),
                Button(main.transform, "VolumeButton", "Volumen", -330, -180, 460, controller.OpenVolume),
                Button(main.transform, "QuitButton", "Salir", -330, -242, 460, controller.Quit)
            };
            var controls = InterfaceTheme.Box(canvas.transform, "Controls", 0, 20, 1040, 570, InterfaceTheme.Panel).gameObject;
            InterfaceTheme.Box(controls.transform, "GoldRule", 0, 284, 1040, 2, InterfaceTheme.Gold);
            var title = InterfaceTheme.MenuText(controls.transform, "PageTitle", "", 36, 0, 215, 920, 70); title.color = InterfaceTheme.Gold;
            var body = InterfaceTheme.MenuText(controls.transform, "Instructions", "", 24, 0, 18, 920, 315, TextAnchor.UpperLeft);
            var footer = InterfaceTheme.MenuText(controls.transform, "PageFooter", "", 18, -190, -215, 520, 50); footer.color = InterfaceTheme.Muted;
            var back = Button(controls.transform, "ControlsBackButton", "Volver", 323, -215, 270, controller.CloseControls);
            var help = InterfaceTheme.MenuText(canvas.transform, "DeviceHelp", "", 17, 0, -330, 1240, 45, TextAnchor.MiddleCenter); help.color = InterfaceTheme.Muted;
            var indicator = InterfaceTheme.Icon(canvas.transform, "keyboard", -600, -330, 24, InterfaceTheme.Gold);
            controller.Configure(input, module, main, controls, null, buttons[0].gameObject, back.gameObject, null, false);
            controller.ConfigureFinal(buttons, title, body, footer, help, indicator); controls.SetActive(false);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        private static void ReplaceMarker(GameObject go, bool actor)
        {
            Directory.CreateDirectory("Assets/UI/H08");
            string path = "Assets/UI/H08/" + (actor ? "ActorDiamond" : "TargetRing") + ".asset";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (mesh == null) { mesh = new Mesh { name = actor ? "ActorDiamond" : "TargetRing" }; AssetDatabase.CreateAsset(mesh, path); }
            int count = actor ? 4 : 48; var vertices = new Vector3[count * 2]; var triangles = new int[count * 6];
            for (int i = 0; i < count; i++)
            {
                float a = (float)i / count * Mathf.PI * 2; var p = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
                vertices[i * 2] = p * .5f; vertices[i * 2 + 1] = p * .425f;
                int n = (i + 1) % count * 2, t = i * 6;
                triangles[t] = i * 2; triangles[t + 1] = i * 2 + 1; triangles[t + 2] = n;
                triangles[t + 3] = n; triangles[t + 4] = i * 2 + 1; triangles[t + 5] = n + 1;
            }
            mesh.Clear(); mesh.vertices = vertices; mesh.triangles = triangles; mesh.RecalculateNormals(); mesh.RecalculateBounds();
            go.GetComponent<MeshFilter>().sharedMesh = mesh; EditorUtility.SetDirty(mesh);
        }

        public static void GenerateIcons()
        {
            const string dir = "Assets/Resources/Interface"; Directory.CreateDirectory(dir);
            foreach (string id in IconNames)
            {
                const int size = 128; var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
                var pixels = new Color32[size * size];
                for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
                {
                    int hits = 0;
                    for (int sy = 0; sy < 2; sy++) for (int sx = 0; sx < 2; sx++)
                        if (Pixel(id, (x + (sx + .5f) / 2) / size * 2 - 1, (y + (sy + .5f) / 2) / size * 2 - 1)) hits++;
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(hits * 255 / 4));
                }
                texture.SetPixels32(pixels); texture.Apply(); string path = dir + "/" + id + ".png";
                File.WriteAllBytes(path, texture.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true; importer.mipmapEnabled = false; importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 128; importer.spritePixelsPerUnit = 128;
                importer.SaveAndReimport();
            }
        }
        // Editable vector primitives are rasterized at 2× coverage for consistent sprite edges.
        private static bool R(float x, float y, float cx, float cy, float w, float h) => Mathf.Abs(x - cx) <= w / 2 && Mathf.Abs(y - cy) <= h / 2;
        private static bool C(float x, float y, float cx, float cy, float r) => (x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r;
        private static bool L(float x, float y, float ax, float ay, float bx, float by, float width)
        { var p = new Vector2(x - ax, y - ay); var v = new Vector2(bx - ax, by - ay); return (p - v * Mathf.Clamp01(Vector2.Dot(p, v) / v.sqrMagnitude)).sqrMagnitude < width * width / 4; }
        private static bool Pixel(string id, float x, float y)
        {
            if (id == "bar") return true;
            if (id == "life") return C(x, y, -.27f, .24f, .34f) || C(x, y, .27f, .24f, .34f) || y < .25f && y > -.72f && Mathf.Abs(x) < (y + .75f) * .8f;
            if (id == "heal") return R(x, y, 0, 0, .36f, 1.4f) || R(x, y, 0, 0, 1.4f, .36f);
            if (id == "mana" || id == "ether") return Mathf.Abs(x) / .6f + Mathf.Abs(y) / .84f < 1 && (id == "mana" || Mathf.Abs(x) / .28f + Mathf.Abs(y) / .4f > 1);
            if (id == "poison") return y < .1f ? C(x, y, 0, -.23f, .49f) : y < .87f && Mathf.Abs(x) < (.88f - y) * .62f;
            if (id == "haste" || id == "tonic") return y > -.1f && y < .8f && Mathf.Abs(x) < (.84f - y) * .8f || R(x, y, 0, -.35f, .36f, .8f);
            if (id == "atb") return C(x, y, 0, 0, .76f) && !C(x, y, 0, 0, .61f) || L(x, y, 0, 0, 0, .47f, .14f) || L(x, y, 0, 0, .35f, -.16f, .14f);
            if (id == "recovery") return R(x,y,0,.7f,1.2f,.14f) || R(x,y,0,-.7f,1.2f,.14f) || L(x,y,-.5f,.6f,.5f,-.6f,.15f) || L(x,y,.5f,.6f,-.5f,-.6f,.15f);
            if (id == "sword" || id == "heavy") return L(x, y, -.55f, -.65f, .55f, .64f, id == "heavy" ? .35f : .19f) || L(x, y, -.55f, -.04f, .03f, -.57f, .16f) || C(x, y, .58f, .67f, .12f);
            if (id == "magic" || id == "storm") return Mathf.Abs(x) < .13f && Mathf.Abs(y) < .8f || Mathf.Abs(y) < .13f && Mathf.Abs(x) < .8f || L(x,y,-.5f,-.5f,.5f,.5f,.13f) || L(x,y,-.5f,.5f,.5f,-.5f,.13f) || id == "storm" && C(x,y,0,0,.9f) && !C(x,y,0,0,.8f);
            if (id == "corrosion") return y < .65f && y > -.75f && Mathf.Abs(x) < .63f * (y + 1.3f) && Mathf.Abs(x) < .65f && Mathf.Abs(x + y * .22f) > .12f;
            if (id == "potion" || id == "flask" || id == "antidote")
            {
                bool bottle = R(x,y,0,.6f,.46f,.4f) || R(x,y,0,-.15f,1.1f,.98f);
                if (id == "antidote") return bottle && !(R(x,y,0,-.15f,.2f,.65f) || R(x,y,0,-.15f,.65f,.2f));
                if (id == "flask") return bottle && !L(x,y,-.35f,-.46f,.35f,.17f,.19f);
                return bottle && !R(x,y,0,-.1f,.65f,.16f);
            }
            if (id == "target") return C(x,y,0,0,.68f) && !C(x,y,0,0,.51f) || R(x,y,0,0,.13f,1.8f) || R(x,y,0,0,1.8f,.13f);
            if (id == "keyboard") return R(x,y,0,0,1.7f,1.1f) && !(Mathf.Abs(y) < .22f && Mathf.Repeat(x+.8f,.32f)<.13f);
            if (id == "gamepad") return (R(x,y,0,0,1.3f,.72f) || C(x,y,-.55f,-.18f,.3f) || C(x,y,.55f,-.18f,.3f)) && !(R(x,y,-.4f,0,.34f,.12f) || R(x,y,-.4f,0,.12f,.34f) || C(x,y,.4f,0,.12f));
            if (id == "fortress") return (R(x,y,0,-.05f,1.5f,1.1f) || R(x,y,-.65f,.55f,.3f,.5f) || R(x,y,.65f,.55f,.3f,.5f) || R(x,y,0,.6f,.3f,.6f)) && !(R(x,y,0,-.4f,.4f,.5f) || C(x,y,0,-.15f,.2f));
            if (id == "mage") return y > .05f && y < .9f && Mathf.Abs(x) < (.95f-y)*.75f || R(x,y,0,.02f,1.5f,.15f) || C(x,y,0,-.3f,.35f) || R(x,y,0,-.7f,1,.25f);
            if (id == "warrior") return (C(x,y,0,.15f,.6f) || R(x,y,0,-.3f,1.2f,.8f)) && !(R(x,y,0,0,1,.16f) || R(x,y,0,-.55f,.16f,.6f));
            if (id == "ogre") return (C(x,y,0,.05f,.64f) || C(x,y,-.65f,.25f,.23f) || C(x,y,.65f,.25f,.23f)) && !(R(x,y,-.25f,.15f,.2f,.14f) || R(x,y,.25f,.15f,.2f,.14f) || R(x,y,0,-.3f,.65f,.14f));
            if (id == "shaman") return L(x,y,-.7f,.75f,0,-.65f,.16f) || L(x,y,.7f,.75f,0,-.65f,.16f) || C(x,y,0,.13f,.4f) && !(C(x,y,-.15f,.15f,.1f) || C(x,y,.15f,.15f,.1f));
            return false;
        }
    }
}
