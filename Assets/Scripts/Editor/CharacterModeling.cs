using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace UltimaGuardia.Editor
{
    /// <summary>
    /// Editable source for the four H05 characters. Metres, Y up, Z forward, ground pivot.
    /// All surfaces are authored polygon lofts; no Unity primitives or external assets.
    /// Rigid meshes are weighted 100% to their parent joint (ordinary Transform hierarchy).
    /// Run only explicitly: importing this source never changes scenes or assets.
    /// </summary>
    public static class CharacterModeling
    {
        public const string ModelFolder = "Assets/Models/H05";
        public const string PrefabFolder = "Assets/Prefabs/H05";
        private static string currentId;
        private static readonly Dictionary<string, Material> palette = new Dictionary<string, Material>();
        private static readonly List<Mesh> transient = new List<Mesh>();
        private static Vector3 V(float x, float y, float z) => new Vector3(x, y, z);
        // Loft section: height, horizontal radius, depth radius.
        private static Vector3 R(float y, float x, float z) => V(y, x, z);

        [MenuItem("Ultima Guardia/H05/Crear modelos e integrar personajes")]
        public static void BuildAndIntegrate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Detener Play antes de integrar H05.");
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.path != "Assets/Scenes/Combat.unity" || scene.isDirty)
                throw new InvalidOperationException("Abrir Combat y guardar cambios antes de integrar H05.");
            var controller = Object.FindFirstObjectByType<CombatPrototype>();
            if (controller == null) throw new InvalidOperationException("Falta CombatController.");
            var oldViews = Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None);
            if (oldViews.Length != 4) throw new InvalidOperationException("Se requieren cuatro vistas existentes.");
            Directory.CreateDirectory(ModelFolder + "/Meshes");
            Directory.CreateDirectory(ModelFolder + "/Exports");
            Directory.CreateDirectory(PrefabFolder);
            Directory.CreateDirectory("Assets/Materials/H05");
            palette.Clear();
            var replacements = new Dictionary<string, CombatantView>();
            foreach (var slot in controller.Encounter.participants)
            {
                currentId = slot.combatant.id;
                var previous = oldViews.Single(v => v.combatantId == currentId);
                var authored = Build(slot.combatant, previous);
                var prefab = PrefabUtility.SaveAsPrefabAsset(authored, PrefabFolder + "/" + currentId + ".prefab");
                Object.DestroyImmediate(authored);
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, previous.transform.parent);
                instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                replacements.Add(currentId, instance.GetComponent<CombatantView>());
                slot.combatant.prefab = prefab;
                EditorUtility.SetDirty(slot.combatant);
            }
            var serialized = new SerializedObject(controller);
            var views = serialized.FindProperty("views");
            for (int i = 0; i < views.arraySize; i++)
            {
                var previous = (CombatantView)views.GetArrayElementAtIndex(i).objectReferenceValue;
                views.GetArrayElementAtIndex(i).objectReferenceValue = replacements[previous.combatantId];
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            foreach (var old in oldViews) Object.DestroyImmediate(old.gameObject);
            // Reserve space above the models for all three temporary status rows.
            var camera = Camera.main;
            camera.fieldOfView = 50;
            camera.transform.LookAt(new Vector3(0, .1f, 2));
            PlayerSettings.bundleVersion = "0.0.6";
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.Refresh();
            ConfigureExports();
            Debug.Log("H05: cuatro modelos propios, armas, jerarquías y prefabs integrados. Reglas y arena conservadas.");
        }

        private static GameObject Build(CombatantData data, CombatantView previous)
        {
            bool ogre = data.id == "ogre", mage = data.id == "mage", shaman = data.id == "shaman";
            var root = new GameObject(data.displayName + "_H05");
            var view = root.AddComponent<CombatantView>(); view.combatantId = data.id;
            var visual = Joint(root.transform, "Visual", Vector3.zero);
            // Three-quarter stance towards opponents and towards the fixed combat camera.
            visual.localRotation = Quaternion.Euler(0, data.team == CombatTeam.Allies ? 20 : -20, 0);
            view.visual = visual;
            var rig = visual.gameObject.AddComponent<CharacterRig>(); rig.definition = data;
            float hipY = ogre ? 1.22f : shaman ? 0.85f : 0.94f;
            float legX = ogre ? 0.39f : shaman ? 0.19f : 0.21f;
            float shoulderY = ogre ? 1.0f : shaman ? 0.61f : 0.65f;
            float shoulderX = ogre ? 0.7f : shaman ? 0.37f : mage ? 0.31f : 0.43f;
            float arm = ogre ? 0.49f : shaman ? 0.33f : 0.34f;
            rig.hips = Joint(visual, "Hips", V(0, hipY, 0));
            rig.chest = Joint(rig.hips, "Chest", V(0, 0.14f, 0));
            rig.head = Joint(rig.chest, "Head", V(0, shoulderY + 0.1f, 0.015f));
            rig.rightUpperArm = Joint(rig.chest, "UpperArm_R", V(shoulderX, shoulderY, 0));
            rig.leftUpperArm = Joint(rig.chest, "UpperArm_L", V(-shoulderX, shoulderY, 0));
            rig.rightForearm = Joint(rig.rightUpperArm, "Forearm_R", V(0, -arm, 0));
            rig.leftForearm = Joint(rig.leftUpperArm, "Forearm_L", V(0, -arm, 0));
            rig.rightHand = Joint(rig.rightForearm, "Hand_R", V(0, -arm * 0.9f, 0));
            rig.leftHand = Joint(rig.leftForearm, "Hand_L", V(0, -arm * 0.9f, 0));
            rig.rightThigh = Joint(rig.hips, "Thigh_R", V(legX, -0.03f, 0));
            rig.leftThigh = Joint(rig.hips, "Thigh_L", V(-legX, -0.03f, 0));
            float leg = (hipY - 0.15f) * 0.5f;
            rig.rightShin = Joint(rig.rightThigh, "Shin_R", V(0, -leg, 0));
            rig.leftShin = Joint(rig.leftThigh, "Shin_L", V(0, -leg, 0));
            rig.rightFoot = Joint(rig.rightShin, "Foot_R", V(0, -leg, 0));
            rig.leftFoot = Joint(rig.leftShin, "Foot_L", V(0, -leg, 0));
            rig.weaponGrip = Joint(rig.rightHand, "WeaponGrip_R", V(0, -0.015f, 0.08f));
            rig.weapon = Joint(rig.weaponGrip, "Weapon", Vector3.zero);
            rig.castOrigin = Joint(rig.weapon, "CastOrigin", Vector3.zero);
            rig.itemOrigin = Joint(rig.leftHand, "ItemOrigin", V(0, 0, 0.13f));
            rig.hitAnchor = Joint(rig.chest, "HitAnchor", V(0, shoulderY * 0.5f, 0));

            if (ogre) Ogre(rig, arm, leg);
            else if (mage) Mage(rig, arm, leg);
            else if (shaman) Shaman(rig, arm, leg);
            else Warrior(rig, arm, leg);
            // Keep the weapon upright while the elbow is flexed in the authored rest pose.
            rig.rightHand.localRotation = Quaternion.Inverse(rig.rightHand.parent.rotation) * visual.rotation * Quaternion.Euler(12, 0, 0);
            rig.joints = new[] { rig.hips, rig.chest, rig.head, rig.rightUpperArm, rig.rightForearm, rig.rightHand,
                rig.leftUpperArm, rig.leftForearm, rig.leftHand, rig.rightThigh, rig.rightShin, rig.rightFoot,
                rig.leftThigh, rig.leftShin, rig.leftFoot, rig.weaponGrip };
            rig.CaptureRestPose();
            MergeMeshes(visual);
            // Keep the existing marker artwork; H08 owns the final marker design.
            view.actorMarker = Object.Instantiate(previous.actorMarker, root.transform);
            view.actorMarker.name = "ActorMarker"; view.actorMarker.SetActive(false);
            view.targetMarker = Object.Instantiate(previous.targetMarker, root.transform);
            view.targetMarker.name = "TargetMarker"; view.targetMarker.SetActive(false);
            if (ogre) { view.actorMarker.transform.localScale = V(2, 0.015f, 2); view.targetMarker.transform.localScale = V(2.2f, 0.015f, 2.2f); }
            view.nameLabel = Object.Instantiate(previous.nameLabel, root.transform);
            view.nameLabel.name = "Name";
            float top = visual.GetComponentsInChildren<Renderer>().Max(r => r.bounds.max.y);
            view.nameLabel.transform.localPosition = V(0, top + 0.16f, 0);
            view.projectileOrigin = rig.castOrigin; view.effectAnchor = rig.hitAnchor;
            Export(root, rig);
            return root;
        }

        private static Material Mat(string key, string hex, float metallic = 0, float smoothness = 0.32f)
        {
            if (palette.TryGetValue(key, out var found)) return found;
            string path = "Assets/Materials/H05/" + key + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) { mat = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(mat, path); }
            ColorUtility.TryParseHtmlString("#" + hex, out var color);
            mat.SetColor("_BaseColor", color); mat.SetFloat("_Metallic", metallic); mat.SetFloat("_Smoothness", smoothness);
            EditorUtility.SetDirty(mat); palette.Add(key, mat); return mat;
        }
        private static Material Gold => Mat("Brass", "DDB366", 0.55f);
        private static Material Dark => Mat("Ink", "24283D");
        private static Material Leather => Mat("Leather", "553C38");
        private static Material Steel => Mat("Steel", "B6C9D1", 0.7f, 0.48f);
        private static Material Bone => Mat("Ivory", "E9D6A8");
        private static Material Skin => Mat("Skin", "C88E70");
        private static Material Blue => Mat("GuardBlue", "356C91");
        private static Material Violet => Mat("ArcaneViolet", "654B97");
        private static Material Cyan => Mat("ArcaneCrystal", "69D3E0", 0.25f, 0.65f);

        private static void Warrior(CharacterRig b, float arm, float leg)
        {
            var iron = Mat("GuardPlate", "779BAE", 0.6f);
            Surface(b.hips, "Tunic", Blue, Loft(8, R(-0.27f, .3f, .21f), R(.05f, .29f, .2f)), V(0, 0, 0));
            Surface(b.chest, "Breastplate", iron, Loft(8, R(-.09f, .28f, .2f), R(.2f, .36f, .245f), R(.51f, .41f, .23f), R(.6f, .28f, .18f)), Vector3.zero);
            Surface(b.chest, "Collar", Gold, Loft(8, R(.57f, .24f, .18f), R(.62f, .24f, .18f)), Vector3.zero);
            Belt(b.hips, .31f, .22f);
            // Raised heraldic kite, shared with the sword pommel shape language.
            Surface(b.chest, "HeraldicKite", Gold, Gem(), V(0, .31f, .249f), V(0, 0, 0), V(.085f, .19f, .025f));
            Surface(b.chest, "KiteInset", Blue, Gem(), V(0, .31f, .276f), Vector3.zero, V(.046f, .12f, .015f));
            for (int side = -1; side <= 1; side += 2)
            {
                var upper = side > 0 ? b.rightUpperArm : b.leftUpperArm;
                var lower = side > 0 ? b.rightForearm : b.leftForearm;
                var hand = side > 0 ? b.rightHand : b.leftHand;
                var thigh = side > 0 ? b.rightThigh : b.leftThigh;
                var shin = side > 0 ? b.rightShin : b.leftShin;
                var foot = side > 0 ? b.rightFoot : b.leftFoot;
                Limb(upper, "Sleeve", Blue, arm, .115f, .1f);
                Surface(upper, "Pauldron", iron, Loft(8, R(-.16f, .19f, .19f), R(.03f, .22f, .21f), R(.12f, .12f, .14f)), Vector3.zero);
                Surface(upper, "PauldronRim", Gold, Loft(8, R(-.17f, .194f, .195f), R(-.125f, .194f, .195f)), Vector3.zero);
                Limb(lower, "Vambrace", iron, arm * .9f, .13f, .09f);
                Surface(lower, "Cuff", Gold, Loft(8, R(-arm * .8f, .115f, .105f), R(-arm * .7f, .115f, .105f)), Vector3.zero);
                Hand(hand, Leather, .85f, true);
                Limb(thigh, "Leggings", Dark, leg, .15f, .12f);
                Surface(thigh, "Tasset", iron, Loft(6, R(-.27f, .16f, .08f), R(0, .14f, .075f)), V(0, -.02f, .13f));
                Limb(shin, "Greave", iron, leg, .14f, .1f);
                Surface(shin, "Knee", Gold, Gem(), V(0, -.02f, .12f), Vector3.zero, V(.11f, .12f, .065f));
                Boot(foot, Leather, .14f, .26f);
            }
            Face(b.head, Skin, .23f, .29f, false);
            Surface(b.head, "Helmet", iron, Loft(10, R(.12f, .258f, .231f), R(.37f, .245f, .22f), R(.44f, .13f, .15f)), V(0, 0, -.025f));
            Surface(b.head, "BrowBand", Gold, Loft(10, R(.105f, .263f, .235f), R(.15f, .263f, .235f)), V(0, 0, -.025f));
            for (int s = -1; s <= 1; s += 2)
                Surface(b.head, "CheekGuard", iron, Loft(4, R(-.13f, .055f, .12f), R(.17f, .065f, .14f)), V(s * .205f, 0, -.025f));
            Surface(b.head, "Crest", Blue, Loft(4, R(.39f, .052f, .17f), R(.62f, .045f, .12f), R(.69f, .005f, .04f)), V(0, 0, -.04f));
            Sword(b.weapon, false); b.castOrigin.localPosition = V(0, 1.11f, 0);
            b.rightUpperArm.localRotation = Quaternion.Euler(-15, 0, -12);
            b.rightForearm.localRotation = Quaternion.Euler(-34, 0, 0);
            b.leftUpperArm.localRotation = Quaternion.Euler(7, 0, 10);
            b.leftForearm.localRotation = Quaternion.Euler(-12, 0, 0);
        }

        private static void Mage(CharacterRig b, float arm, float leg)
        {
            var robe = Mat("ArcaneRobe", "3C365E");
            var hair = Mat("MageHair", "DBBD91");
            Robe(b, robe, Violet, leg, .32f, .23f);
            Surface(b.chest, "Bodice", Violet, Loft(8, R(-.1f, .22f, .16f), R(.18f, .25f, .18f), R(.46f, .29f, .18f), R(.58f, .2f, .14f)), Vector3.zero);
            Surface(b.chest, "Mantle", robe, Loft(10, R(.38f, .39f, .23f), R(.53f, .38f, .23f), R(.64f, .19f, .15f)), V(0, 0, -.025f));
            Surface(b.chest, "MantleTrim", Gold, Loft(10, R(.37f, .39f, .232f), R(.405f, .39f, .232f)), V(0, 0, -.025f));
            Belt(b.hips, .255f, .18f);
            Surface(b.chest, "Amulet", Cyan, Gem(), V(0, .4f, .23f), Vector3.zero, V(.07f, .1f, .04f));
            Arms(b, robe, Skin, arm, .095f);
            foreach (var upper in new[] { b.rightUpperArm, b.leftUpperArm })
                Surface(upper, "SleeveCap", Violet, Loft(8, R(-.19f, .145f, .145f), R(.03f, .13f, .13f)), Vector3.zero);
            foreach (var lower in new[] { b.rightForearm, b.leftForearm })
            {
                Surface(lower, "FlaredSleeve", Violet, Loft(8, R(-arm * .75f, .15f, .14f), R(-.06f, .105f, .11f)), Vector3.zero);
                Surface(lower, "GoldCuff", Gold, Loft(8, R(-arm * .78f, .155f, .145f), R(-arm * .68f, .155f, .145f)), Vector3.zero);
            }
            Face(b.head, Skin, .205f, .28f, false);
            Surface(b.head, "HairCap", hair, Loft(10, R(.1f, .222f, .21f), R(.31f, .21f, .19f), R(.38f, .12f, .12f)), V(0, 0, -.025f));
            for (int s = -1; s <= 1; s += 2)
                Surface(b.head, "HairLock", hair, Loft(5, R(-.27f, .06f, .06f), R(.19f, .075f, .09f)), V(s * .19f, 0, -.03f));
            Surface(b.head, "HatBrim", robe, Loft(12, R(.28f, .4f, .35f), R(.34f, .4f, .35f)), Vector3.zero);
            Surface(b.head, "HatCrown", Violet, Loft(10, R(.34f, .26f, .23f), R(.6f, .17f, .15f), R(.83f, .055f, .06f), R(.89f, 0, 0)), Vector3.zero, V(0, 0, -10));
            Surface(b.head, "HatBand", Gold, Loft(10, R(.35f, .265f, .235f), R(.41f, .25f, .22f)), Vector3.zero);
            Surface(b.head, "HatJewel", Cyan, Gem(), V(0, .4f, .24f), Vector3.zero, V(.07f, .08f, .035f));
            Staff(b.weapon, false); b.castOrigin.localPosition = V(0, 1.18f, 0);
            b.rightUpperArm.localRotation = Quaternion.Euler(-13, 0, -12);
            b.rightForearm.localRotation = Quaternion.Euler(-55, 0, 0);
            b.leftUpperArm.localRotation = Quaternion.Euler(-16, 0, 14);
            b.leftForearm.localRotation = Quaternion.Euler(-30, 0, 0);
        }

        private static void Ogre(CharacterRig b, float arm, float leg)
        {
            var skin = Mat("OgreSkin", "879E67");
            var iron = Mat("RaiderIron", "626C70", .5f);
            var rust = Mat("RaiderCloth", "8D4639");
            Surface(b.hips, "Loincloth", rust, Loft(8, R(-.38f, .48f, .32f), R(.12f, .43f, .29f)), Vector3.zero);
            Surface(b.chest, "Body", skin, Loft(10, R(-.15f, .42f, .3f), R(.2f, .59f, .36f), R(.66f, .69f, .37f), R(.99f, .6f, .31f), R(1.08f, .35f, .25f)), Vector3.zero);
            Belt(b.hips, .48f, .32f);
            Surface(b.chest, "Harness", Leather, Loft(4, R(-.15f, .1f, .055f), R(1.06f, .1f, .055f)), V(.18f, 0, .33f), V(0, 0, -21));
            Surface(b.chest, "HarnessStud", Steel, Gem(), V(.13f, .61f, .402f), Vector3.zero, V(.08f, .09f, .035f));
            Arms(b, skin, skin, arm, .195f);
            foreach (var lower in new[] { b.rightForearm, b.leftForearm })
                Surface(lower, "Bracer", Leather, Loft(8, R(-arm * .82f, .19f, .18f), R(-.13f, .23f, .2f)), Vector3.zero);
            Surface(b.leftUpperArm, "ScavengedPauldron", iron, Loft(8, R(-.23f, .3f, .28f), R(.09f, .32f, .3f), R(.2f, .16f, .17f)), Vector3.zero);
            for (int i = -1; i <= 1; i++)
                Surface(b.leftUpperArm, "PauldronSpike", Bone, Loft(5, R(0, .075f, .075f), R(.29f, 0, 0)), V(i * .17f, .12f, 0), V(0, 0, -i * 23));
            Legs(b, skin, Leather, leg, .23f, .21f, .36f);
            foreach (var shin in new[] { b.rightShin, b.leftShin })
                Surface(shin, "LegWrap", rust, Loft(8, R(-leg * .8f, .2f, .19f), R(-leg * .35f, .22f, .2f)), Vector3.zero);
            Face(b.head, skin, .36f, .38f, true);
            Surface(b.head, "Jaw", skin, Loft(8, R(-.25f, .3f, .23f), R(-.05f, .36f, .29f), R(.04f, .32f, .24f)), V(0, 0, .06f));
            for (int s = -1; s <= 1; s += 2)
            {
                Surface(b.head, "Tusk", Bone, Loft(6, R(0, .065f, .055f), R(.17f, .038f, .035f), R(.25f, 0, 0)), V(s * .225f, -.12f, .31f), V(-12, 0, s * -17));
                Surface(b.head, "Ear", skin, Gem(), V(s * .4f, .13f, 0), V(0, 0, s * -65), V(.13f, .22f, .09f));
            }
            Surface(b.head, "Mohawk", rust, Loft(4, R(.29f, .09f, .22f), R(.49f, .065f, .17f), R(.54f, .01f, .07f)), V(0, 0, -.02f));
            Sword(b.weapon, true); b.castOrigin.localPosition = V(0, 1.22f, 0);
            b.rightUpperArm.localRotation = Quaternion.Euler(-12, 0, -13);
            b.rightForearm.localRotation = Quaternion.Euler(-32, 0, 0);
            b.leftUpperArm.localRotation = Quaternion.Euler(10, 0, 18);
            b.leftForearm.localRotation = Quaternion.Euler(-10, 0, 0);
        }

        private static void Shaman(CharacterRig b, float arm, float leg)
        {
            var skin = Mat("ShamanSkin", "9C8563");
            var robe = Mat("ShamanRobe", "714942");
            var trim = Mat("ShamanMantle", "BE8C52");
            var poison = Mat("VenomCrystal", "95CE70", .2f, .55f);
            Robe(b, robe, trim, leg, .31f, .24f);
            Surface(b.chest, "Tunic", robe, Loft(8, R(-.11f, .25f, .19f), R(.4f, .32f, .22f), R(.58f, .22f, .16f)), Vector3.zero);
            Surface(b.chest, "FurMantle", trim, Loft(10, R(.32f, .4f, .25f), R(.52f, .42f, .25f), R(.68f, .21f, .16f)), V(0, 0, -.025f));
            for (int i = -2; i <= 2; i++)
                Surface(b.chest, "BonePendant", Bone, Loft(5, R(-.12f, 0, 0), R(0, .034f, .03f), R(.09f, .025f, .02f)), V(i * .1f, .39f - Mathf.Abs(i) * .025f, .25f));
            Belt(b.hips, .28f, .21f);
            Arms(b, skin, skin, arm, .105f);
            foreach (var lower in new[] { b.rightForearm, b.leftForearm })
                Surface(lower, "RitualWrap", robe, Loft(7, R(-arm * .8f, .13f, .12f), R(-.08f, .13f, .12f)), Vector3.zero);
            Face(b.head, skin, .23f, .28f, false);
            // Open-faced carved mask, with amber eye slits and a clearly separate mouth.
            Surface(b.head, "RitualMask", Bone, Loft(6, R(-.03f, .21f, .035f), R(.22f, .27f, .05f), R(.4f, .14f, .03f)), V(0, .02f, .195f));
            for (int s = -1; s <= 1; s += 2)
            {
                Surface(b.head, "MaskEye", Dark, Loft(4, R(-.025f, .075f, .012f), R(.025f, .075f, .012f)), V(s * .103f, .18f, .249f), V(0, 0, s * -17));
                Surface(b.head, "Horn", Bone, Loft(6, R(0, .075f, .065f), R(.25f, .055f, .045f), R(.43f, 0, 0)), V(s * .2f, .32f, -.04f), V(-12, 0, s * -33));
                Surface(b.head, "Ear", skin, Gem(), V(s * .26f, .04f, -.02f), V(0, 0, s * -65), V(.085f, .18f, .05f));
            }
            Surface(b.head, "MaskSigil", Mat("RitualRed", "9F4E3E"), Gem(), V(0, .32f, .24f), Vector3.zero, V(.045f, .09f, .012f));
            Staff(b.weapon, true); b.castOrigin.localPosition = V(0, 1.12f, 0);
            Surface(b.weapon, "VenomFocus", poison, Gem(), V(0, 1.03f, 0), Vector3.zero, V(.13f, .19f, .1f));
            b.rightUpperArm.localRotation = Quaternion.Euler(-15, 0, -15);
            b.rightForearm.localRotation = Quaternion.Euler(-43, 0, 0);
            b.leftUpperArm.localRotation = Quaternion.Euler(-17, 0, 17);
            b.leftForearm.localRotation = Quaternion.Euler(-35, 0, 0);
        }

        private static void Face(Transform parent, Material skin, float width, float height, bool angry)
        {
            Surface(parent, "HeadShape", skin, Loft(10, R(-height * .6f, width * .65f, width * .65f), R(-height * .2f, width, width * .85f),
                R(height * .55f, width, width * .84f), R(height, width * .62f, width * .62f)), Vector3.zero);
            Surface(parent, "Nose", skin, Gem(), V(0, -.005f, width * .87f), V(15, 0, 0), V(width * .2f, .09f, width * .33f));
            for (int s = -1; s <= 1; s += 2)
            {
                Surface(parent, "EyeSocket", Dark, Loft(4, R(-.03f, .065f, .015f), R(.03f, .065f, .015f)), V(s * width * .44f, .09f, width * .815f), V(0, s * 12, angry ? s * -14 : 0));
                Surface(parent, "Eye", Bone, Loft(4, R(-.013f, .039f, .009f), R(.013f, .039f, .009f)), V(s * width * .44f, .09f, width * .84f));
                Surface(parent, "Pupil", Dark, Gem(), V(s * width * .44f, .09f, width * .87f), Vector3.zero, V(.012f, .021f, .012f));
                Surface(parent, "Brow", skin, Loft(4, R(-.022f, .078f, .025f), R(.022f, .078f, .025f)), V(s * width * .45f, .15f, width * .83f), V(0, s * 10, s * -13));
            }
            Surface(parent, "Mouth", Dark, Loft(4, R(-.012f, width * .3f, .008f), R(.012f, width * .3f, .008f)), V(0, -height * .27f, width * .78f));
        }
        private static void Arms(CharacterRig b, Material sleeve, Material hand, float arm, float radius)
        {
            foreach (var t in new[] { b.rightUpperArm, b.leftUpperArm }) Limb(t, "UpperArm", sleeve, arm, radius * 1.18f, radius);
            foreach (var t in new[] { b.rightForearm, b.leftForearm }) Limb(t, "Forearm", sleeve, arm * .9f, radius, radius * .8f);
            Hand(b.rightHand, hand, radius / .105f, true); Hand(b.leftHand, hand, radius / .105f, false);
        }
        private static void Hand(Transform t, Material mat, float size, bool gripping)
        {
            Surface(t, "Palm", mat, Loft(6, R(-.11f, .075f, .05f), R(.025f, .065f, .05f)), V(0, 0, .015f), Vector3.zero, Vector3.one * size);
            for (int i = 0; i < 4; i++)
                Surface(t, "Finger", mat, Loft(5, R(-.045f, .018f, .02f), R(.025f, .018f, .02f)), V((i - 1.5f) * .034f * size, -.09f * size, (gripping ? .065f : .04f) * size), V(gripping ? 65 : 10, 0, 0), Vector3.one * size);
            Surface(t, "Thumb", mat, Loft(5, R(-.045f, .025f, .025f), R(.025f, .025f, .025f)), V(-.073f * size, -.04f * size, .04f * size), V(25, 0, -35), Vector3.one * size);
        }
        private static void Legs(CharacterRig b, Material pants, Material boots, float leg, float radius, float footX, float footZ)
        {
            foreach (var t in new[] { b.rightThigh, b.leftThigh }) Limb(t, "Thigh", pants, leg, radius * 1.15f, radius);
            foreach (var t in new[] { b.rightShin, b.leftShin }) Limb(t, "Shin", pants, leg, radius, radius * .8f);
            Boot(b.rightFoot, boots, footX, footZ); Boot(b.leftFoot, boots, footX, footZ);
        }
        private static void Robe(CharacterRig b, Material robe, Material trim, float leg, float width, float depth)
        {
            // Separate thigh panels and centre tabard allow hip/knee poses without stretching cloth.
            Legs(b, Dark, Leather, leg, .105f, .115f, .22f);
            Surface(b.hips, "RobeHip", robe, Loft(10, R(-.25f, width, depth), R(.15f, width * .77f, depth * .78f)), Vector3.zero);
            foreach (var thigh in new[] { b.rightThigh, b.leftThigh })
            {
                Surface(thigh, "SplitRobePanel", robe, Loft(8, R(-leg * 1.65f, .22f, depth * 1.1f), R(-.1f, .17f, depth)), V(0, 0, -.015f));
                Surface(thigh, "RobeHem", trim, Loft(8, R(-leg * 1.65f, .223f, depth * 1.11f), R(-leg * 1.52f, .217f, depth * 1.1f)), V(0, 0, -.015f));
            }
            Surface(b.hips, "Tabard", trim, Loft(4, R(-.66f, .09f, .025f), R(-.07f, .14f, .03f)), V(0, 0, depth + .015f));
        }
        private static void Belt(Transform t, float width, float depth)
        {
            Surface(t, "Belt", Leather, Loft(8, R(-.02f, width, depth), R(.09f, width, depth)), Vector3.zero);
            Surface(t, "Buckle", Gold, Loft(4, R(-.055f, .085f, .025f), R(.055f, .085f, .025f)), V(0, .035f, depth));
        }
        private static void Limb(Transform parent, string name, Material mat, float length, float top, float bottom)
            => Surface(parent, name, mat, Loft(8, R(-length, bottom, bottom * .9f), R(-length * .8f, bottom * 1.08f, bottom), R(-.04f, top, top * .9f), R(.04f, top * .85f, top * .8f)), Vector3.zero);
        private static void Boot(Transform t, Material mat, float x, float z)
            => Surface(t, "Boot", mat, Loft(8, R(-.12f, x, z), R(.02f, x, z), R(.16f, x * .72f, z * .55f)), V(0, 0, z * .22f));
        private static void Sword(Transform t, bool heavy)
        {
            Surface(t, "Grip", Leather, Loft(8, R(-.16f, .045f, .04f), R(.16f, .045f, .04f)), Vector3.zero);
            Surface(t, "Pommel", Gold, Gem(), V(0, -.2f, 0), Vector3.zero, V(.075f, .07f, .06f));
            Surface(t, "Guard", heavy ? Steel : Gold, Loft(4, R(-.045f, heavy ? .22f : .24f, .065f), R(.045f, heavy ? .22f : .24f, .065f)), V(0, .18f, 0));
            Surface(t, "Blade", Steel, Loft(4, R(.22f, heavy ? .15f : .085f, heavy ? .06f : .035f), R(heavy ? 1.1f : .95f, heavy ? .21f : .075f, .025f), R(heavy ? 1.3f : 1.12f, 0, 0)), Vector3.zero);
            if (heavy) Surface(t, "BladeSpine", Mat("RaiderIron", "626C70", .5f), Loft(4, R(.22f, .04f, .061f), R(1.1f, .04f, .027f)), V(-.1f, 0, 0));
        }
        private static void Staff(Transform t, bool ritual)
        {
            Surface(t, "Shaft", Leather, Loft(8, R(-.68f, .035f, .035f), R(.83f, .045f, .045f)), Vector3.zero);
            Surface(t, "Heel", ritual ? Bone : Gold, Loft(8, R(-.72f, .045f, .045f), R(-.59f, .045f, .045f)), Vector3.zero);
            Surface(t, "GripWrap", ritual ? Bone : Gold, Loft(8, R(-.09f, .05f, .05f), R(.12f, .05f, .05f)), Vector3.zero);
            Surface(t, "FocusCollar", ritual ? Bone : Gold, Loft(8, R(.7f, .09f, .08f), R(.85f, .13f, .11f)), Vector3.zero);
            if (!ritual) Surface(t, "Crystal", Cyan, Gem(), V(0, 1.02f, 0), Vector3.zero, V(.13f, .28f, .11f));
            for (int s = -1; s <= 1; s += 2)
                Surface(t, "FocusProng", ritual ? Bone : Gold, Loft(6, R(0, .045f, .04f), R(.22f, .04f, .035f), R(.37f, 0, 0)), V(s * .1f, .79f, 0), V(0, 0, -s * 21));
        }
        private static Transform Joint(Transform parent, string name, Vector3 position)
        { var t = new GameObject(name).transform; t.SetParent(parent, false); t.localPosition = position; return t; }
        private static Mesh Gem() => Loft(6, R(-1, 0, 0), R(0, 1, 1), R(1, 0, 0));
        private static void Surface(Transform parent, string name, Material mat, Mesh mesh, Vector3 pos, Vector3? angles = null, Vector3? scale = null)
        {
            var go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer)); go.transform.SetParent(parent, false);
            go.transform.localPosition = pos; go.transform.localRotation = Quaternion.Euler(angles ?? Vector3.zero); go.transform.localScale = scale ?? Vector3.one;
            go.GetComponent<MeshFilter>().sharedMesh = mesh; go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            transient.Add(mesh);
        }
        private static Mesh Loft(int sides, params Vector3[] rings)
        {
            var vertices = new List<Vector3>(); var indices = new List<int>(); var uv = new List<Vector2>();
            Action<Vector3, Vector3, Vector3> tri = (a, b, c) =>
            {
                if (Vector3.Cross(b - a, c - a).sqrMagnitude < 0.0000000001f) return;
                int start = vertices.Count; vertices.Add(a); vertices.Add(b); vertices.Add(c);
                indices.Add(start); indices.Add(start + 2); indices.Add(start + 1);
                uv.Add(new Vector2(a.x, a.y)); uv.Add(new Vector2(b.x, b.y)); uv.Add(new Vector2(c.x, c.y));
            };
            Func<int, int, Vector3> p = (r, i) =>
            {
                float angle = (i + .5f) * Mathf.PI * 2 / sides;
                return V(Mathf.Sin(angle) * rings[r].y, rings[r].x, Mathf.Cos(angle) * rings[r].z);
            };
            for (int r = 0; r < rings.Length - 1; r++)
                for (int i = 0; i < sides; i++)
                { tri(p(r, i), p(r + 1, i), p(r + 1, i + 1)); tri(p(r, i), p(r + 1, i + 1), p(r, i + 1)); }
            for (int i = 0; i < sides; i++)
            { tri(V(0, rings[0].x, 0), p(0, i), p(0, i + 1)); tri(V(0, rings[rings.Length - 1].x, 0), p(rings.Length - 1, i + 1), p(rings.Length - 1, i)); }
            var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetTriangles(indices, 0); mesh.SetUVs(0, uv); mesh.RecalculateNormals(); mesh.RecalculateBounds(); return mesh;
        }
        private static void MergeMeshes(Transform visual)
        {
            var filters = visual.GetComponentsInChildren<MeshFilter>();
            int index = 0;
            foreach (var group in filters.GroupBy(f => new { parent = f.transform.parent, mat = f.GetComponent<Renderer>().sharedMaterial }))
            {
                var combined = new Mesh { name = currentId + "_" + group.Key.parent.name + "_" + group.Key.mat.name };
                combined.CombineMeshes(group.Select(f => new CombineInstance { mesh = f.sharedMesh, transform = Matrix4x4.TRS(f.transform.localPosition, f.transform.localRotation, f.transform.localScale) }).ToArray(), true, true);
                string path = ModelFolder + "/Meshes/" + currentId + "_" + (index++).ToString("D2") + ".asset";
                var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                if (existing != null) { EditorUtility.CopySerialized(combined, existing); Object.DestroyImmediate(combined); combined = existing; }
                else AssetDatabase.CreateAsset(combined, path);
                var go = new GameObject(combined.name, typeof(MeshFilter), typeof(MeshRenderer)); go.transform.SetParent(group.Key.parent, false);
                go.GetComponent<MeshFilter>().sharedMesh = combined; go.GetComponent<MeshRenderer>().sharedMaterial = group.Key.mat;
                go.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
            }
            foreach (var f in filters) Object.DestroyImmediate(f.gameObject);
            foreach (var mesh in transient) Object.DestroyImmediate(mesh); transient.Clear();
        }
        [Serializable] private sealed class RigExport { public string id, convention = "metres; Y up; Z forward; rigid pieces, 100% parent joint"; public JointExport[] joints; }
        [Serializable] private sealed class JointExport { public string path, parent; public Vector3 position, scale; public Quaternion rotation; }
        private static string Number(float n) => n.ToString("0.######", CultureInfo.InvariantCulture);
        private static void Export(GameObject root, CharacterRig rig)
        {
            var obj = new StringBuilder("# Ultima Guardia H05 - own authored geometry. Metres, Y-up, Z-forward.\n");
            obj.AppendLine("mtllib " + currentId + ".mtl");
            var mtl = new StringBuilder(); var used = new HashSet<Material>(); int offset = 1;
            // Export in model coordinates (without the scene-facing yaw on Visual).
            foreach (var f in rig.GetComponentsInChildren<MeshFilter>())
            {
                var mesh = f.sharedMesh; var mat = f.GetComponent<Renderer>().sharedMaterial;
                var matrix = rig.transform.worldToLocalMatrix * f.transform.localToWorldMatrix;
                obj.AppendLine("o " + f.name); obj.AppendLine("usemtl " + mat.name);
                foreach (var vertex in mesh.vertices) { var p = matrix.MultiplyPoint3x4(vertex); obj.AppendLine("v " + Number(p.x) + " " + Number(p.y) + " " + Number(p.z)); }
                foreach (var normal in mesh.normals) { var n = matrix.MultiplyVector(normal).normalized; obj.AppendLine("vn " + Number(n.x) + " " + Number(n.y) + " " + Number(n.z)); }
                for (int i = 0; i < mesh.triangles.Length; i += 3)
                {
                    int a = offset + mesh.triangles[i], b = offset + mesh.triangles[i + 1], c = offset + mesh.triangles[i + 2];
                    obj.AppendLine("f " + a + "//" + a + " " + b + "//" + b + " " + c + "//" + c);
                }
                offset += mesh.vertexCount;
                if (used.Add(mat)) { var c = mat.color; mtl.AppendLine("newmtl " + mat.name + "\nKd " + Number(c.r) + " " + Number(c.g) + " " + Number(c.b) + "\n"); }
            }
            File.WriteAllText(ModelFolder + "/Exports/" + currentId + ".obj", obj.ToString());
            File.WriteAllText(ModelFolder + "/Exports/" + currentId + ".mtl", mtl.ToString());
            var metadata = new RigExport { id = currentId, joints = rig.GetComponentsInChildren<Transform>().Select(t => new JointExport {
                path = AnimationUtility.CalculateTransformPath(t, rig.transform), parent = t == rig.transform ? "" : AnimationUtility.CalculateTransformPath(t.parent, rig.transform),
                position = t.localPosition, rotation = t.localRotation, scale = t.localScale }).ToArray() };
            File.WriteAllText(ModelFolder + "/Exports/" + currentId + ".rig.json", JsonUtility.ToJson(metadata, true));
        }

        public static void Reexport()
        {
            foreach (string id in new[] { "warrior", "mage", "ogre", "shaman" })
            {
                currentId = id;
                var root = PrefabUtility.LoadPrefabContents(PrefabFolder + "/" + id + ".prefab");
                try { Export(root, root.GetComponentInChildren<CharacterRig>()); }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
            AssetDatabase.Refresh();
            ConfigureExports();
        }

        private static void ConfigureExports()
        {
            foreach (string id in new[] { "warrior", "mage", "ogre", "shaman" })
            {
                string path = ModelFolder + "/Exports/" + id + ".obj";
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
                importer.globalScale = 1; importer.importAnimation = false;
                importer.animationType = ModelImporterAnimationType.None;
                importer.importCameras = false; importer.importLights = false;
                importer.importNormals = ModelImporterNormals.Import;
                importer.preserveHierarchy = true;
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabFolder + "/" + id + ".prefab");
                foreach (var mat in prefab.GetComponentInChildren<CharacterRig>().GetComponentsInChildren<MeshRenderer>().Select(r => r.sharedMaterial).Distinct())
                    importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), mat.name), mat);
                importer.SaveAndReimport();
            }
        }
    }
}
