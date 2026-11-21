using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltimaGuardia.Editor
{
    /// <summary>Editor-only inspection and reproducible still poses; no H06 gameplay clips.</summary>
    public static class CharacterArtValidation
    {
        public const string Evidence = "Documentation/Evidence/H05";
        private static readonly string[] Ids = { "warrior", "mage", "ogre", "shaman" };
        public static readonly string[] PoseNames = { "Reposo", "Ataque", "Conjuro", "Reaccion", "Objeto", "Caida" };
        [Serializable] public sealed class AuditReport { public string version; public bool passed; public string[] checks; public CharacterReport[] characters; }
        [Serializable] public sealed class CharacterReport
        {
            public string id; public int triangles, vertices, renderers, materials, joints;
            public Vector3 size; public float minimumY; public string[] poses;
        }

        [MenuItem("Ultima Guardia/H05/Auditar modelos y capturar hojas")]
        public static void AuditAndCapture()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Guardar Combat y detener Play antes de la auditoría.");
            Directory.CreateDirectory(Evidence);
            var controller = Object.FindFirstObjectByType<CombatPrototype>();
            Require(controller != null, "Abrir Combat antes de auditar.");
            Require(controller.Encounter.ValidateData().Count == 0, "Datos del encuentro válidos.");
            var sceneViews = Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None);
            Require(sceneViews.Length == 4, "Exactamente cuatro vistas.");
            var reports = new List<CharacterReport>();
            foreach (string id in Ids)
            {
                var view = sceneViews.Single(v => v.combatantId == id);
                var rig = view.GetComponentInChildren<CharacterRig>();
                Require(rig != null && rig.definition.id == id, id + ": rig/datos.");
                Require(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(view.gameObject) == CharacterModeling.PrefabFolder + "/" + id + ".prefab", id + ": prefab final.");
                Require(view.projectileOrigin == rig.castOrigin && view.effectAnchor == rig.hitAnchor, id + ": sockets.");
                Require(rig.weapon.parent == rig.weaponGrip && rig.weaponGrip.parent == rig.rightHand, id + ": agarre.");
                Require(rig.itemOrigin.IsChildOf(rig.leftHand), id + ": objeto.");
                Require(rig.joints.Length == 16 && rig.joints.Distinct().Count() == 16, id + ": 16 articulaciones.");
                Require(rig.restPositions.Length == 16 && rig.restRotations.Length == 16, id + ": pose de reposo.");
                Require(view.transform.lossyScale == Vector3.one, id + ": escala de raíz.");
                var filters = rig.GetComponentsInChildren<MeshFilter>();
                var renderers = rig.GetComponentsInChildren<MeshRenderer>();
                Require(filters.Length > 15 && renderers.Length == filters.Length && renderers.Length <= 65, id + ": presupuesto de renderers.");
                int triangles = 0, vertices = 0;
                foreach (var f in filters)
                {
                    var mesh = f.sharedMesh; var mat = f.GetComponent<MeshRenderer>().sharedMaterial;
                    Require(mesh != null && AssetDatabase.GetAssetPath(mesh).StartsWith(CharacterModeling.ModelFolder), id + ": malla guardada.");
                    Require(mat != null && mat.shader.name == "Universal Render Pipeline/Lit", id + ": material URP.");
                    Require(mesh.normals.Length == mesh.vertexCount, id + ": normales.");
                    Require(mesh.vertices.All(v => float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z)), id + ": vértices finitos.");
                    Require(mesh.triangles.All(i => i >= 0 && i < mesh.vertexCount), id + ": índices válidos.");
                    for (int i = 0; i < mesh.triangles.Length; i += 3)
                    {
                        var a = mesh.vertices[mesh.triangles[i]]; var b = mesh.vertices[mesh.triangles[i + 1]]; var c = mesh.vertices[mesh.triangles[i + 2]];
                        Require(Vector3.Cross(b - a, c - a).sqrMagnitude > 1e-12f, id + ": triángulo no degenerado.");
                    }
                    triangles += mesh.triangles.Length / 3; vertices += mesh.vertexCount;
                }
                Require(triangles < 10000, id + ": presupuesto de triángulos.");
                var localBounds = BoundsFor(rig);
                Require(localBounds.min.y >= -.02f && localBounds.min.y <= .03f, id + ": pies sobre suelo.");
                Require(localBounds.size.y > 1.8f && localBounds.size.y < 3.6f, id + ": escala en metros.");
                foreach (string suffix in new[] { ".obj", ".mtl", ".rig.json" }) Require(File.Exists(CharacterModeling.ModelFolder + "/Exports/" + id + suffix), id + ": fuente/exportación " + suffix);
                var imported = AssetDatabase.LoadAssetAtPath<GameObject>(CharacterModeling.ModelFolder + "/Exports/" + id + ".obj");
                Require(imported != null && imported.GetComponentsInChildren<MeshFilter>().Length > 0, id + ": OBJ importado.");
                reports.Add(new CharacterReport { id = id, triangles = triangles, vertices = vertices, renderers = renderers.Length,
                    materials = renderers.Select(r => r.sharedMaterial).Distinct().Count(), joints = rig.joints.Length, size = localBounds.size, minimumY = localBounds.min.y,
                    poses = PoseNames });
            }
            Require(reports.Single(r => r.id == "ogre").size.y > reports.Single(r => r.id == "warrior").size.y, "Ogro más alto que guerrero.");
            var report = new AuditReport { version = Application.version, passed = true, characters = reports.ToArray(), checks = new[] {
                "Four scene instances and data references use H05 prefabs", "16 unique joints with captured local rest transforms each", "Weapons follow right-hand grips; cast/item/hit sockets assigned",
                "All triangles non-degenerate, finite and indexed; saved meshes and URP materials", "Ground contact and metre-scale bounds", "OBJ/MTL and hierarchy source exports present, OBJ imported", "Budget below 10000 triangles and 65 renderers each" } };
            File.WriteAllText(Evidence + "/model-audit.json", JsonUtility.ToJson(report, true));
            CaptureSheets();
            Debug.Log("H05: auditoría estructural y hojas de seis poses por personaje generadas.");
        }

        private static void Require(bool condition, string message)
        { if (!condition) throw new InvalidOperationException("H05 audit: " + message); }
        public static Bounds BoundsFor(CharacterRig rig)
        {
            var points = new List<Vector3>();
            foreach (var f in rig.GetComponentsInChildren<MeshFilter>())
                points.AddRange(f.sharedMesh.vertices.Select(v => rig.transform.InverseTransformPoint(f.transform.TransformPoint(v))));
            var bounds = new Bounds(points[0], Vector3.zero); foreach (var p in points) bounds.Encapsulate(p); return bounds;
        }

        public static void ApplyPose(CharacterRig rig, int pose)
        {
            rig.RestoreRestPose();
            switch (pose)
            {
                case 1:
                    rig.chest.localRotation = Quaternion.Euler(12, -25, 0);
                    rig.rightUpperArm.localRotation = Quaternion.Euler(-105, 0, -20);
                    rig.rightForearm.localRotation = Quaternion.Euler(-65, 0, 0);
                    rig.leftUpperArm.localRotation = Quaternion.Euler(-20, 0, 25);
                    rig.rightThigh.localRotation = Quaternion.Euler(-20, 0, 0);
                    rig.rightShin.localRotation = Quaternion.Euler(25, 0, 0);
                    rig.rightHand.rotation = rig.transform.rotation * Quaternion.Euler(60, 0, -35);
                    break;
                case 2:
                    rig.rightUpperArm.localRotation = Quaternion.Euler(-145, 0, -18);
                    rig.rightForearm.localRotation = Quaternion.Euler(-25, 0, 0);
                    rig.leftUpperArm.localRotation = Quaternion.Euler(-75, 0, 28);
                    rig.leftForearm.localRotation = Quaternion.Euler(-35, 0, 0);
                    rig.head.localRotation = Quaternion.Euler(-15, 0, 0);
                    rig.rightHand.rotation = rig.transform.rotation * Quaternion.Euler(8, 0, -8);
                    break;
                case 3:
                    rig.chest.localRotation = Quaternion.Euler(-22, 0, 12);
                    rig.head.localRotation = Quaternion.Euler(-12, 15, 0);
                    rig.leftUpperArm.localRotation = Quaternion.Euler(-35, 0, 45);
                    rig.leftForearm.localRotation = Quaternion.Euler(-85, 0, 0);
                    break;
                case 4:
                    rig.leftUpperArm.localRotation = Quaternion.Euler(-70, 0, 15);
                    rig.leftForearm.localRotation = Quaternion.Euler(-90, 0, 0);
                    rig.head.localRotation = Quaternion.Euler(12, -25, 0);
                    break;
                case 5:
                    rig.hips.localPosition = new Vector3(0, .48f, 0);
                    rig.hips.localRotation = Quaternion.Euler(0, 0, 85);
                    rig.chest.localRotation = Quaternion.Euler(0, 0, -5);
                    rig.rightUpperArm.localRotation = Quaternion.Euler(-10, 0, -50);
                    rig.rightForearm.localRotation = Quaternion.Euler(-60, 0, 0);
                    rig.leftUpperArm.localRotation = Quaternion.Euler(-70, 0, 80);
                    rig.leftForearm.localRotation = Quaternion.Euler(-30, 0, 0);
                    rig.rightThigh.localRotation = Quaternion.Euler(-20, 0, -12);
                    rig.rightShin.localRotation = Quaternion.Euler(45, 0, 0);
                    break;
            }
        }

        private static void CaptureSheets()
        {
            string original = EditorSceneManager.GetActiveScene().path;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            try
            {
                RenderSettings.ambientLight = new Color(.6f, .64f, .72f);
                var camera = new GameObject("ArtCamera", typeof(Camera)).GetComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.055f, .075f, .105f);
                camera.orthographic = true; camera.nearClipPlane = .1f; camera.farClipPlane = 50;
                var light = new GameObject("Key", typeof(Light)).GetComponent<Light>();
                light.type = LightType.Directional; light.intensity = 1.6f; light.transform.rotation = Quaternion.Euler(40, -35, 0); light.shadows = LightShadows.Soft;
                var fill = new GameObject("Fill", typeof(Light)).GetComponent<Light>(); fill.type = LightType.Directional; fill.intensity = .65f; fill.transform.rotation = Quaternion.Euler(20, 140, 0);
                var instances = new List<GameObject>();
                for (int i = 0; i < Ids.Length; i++)
                {
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterModeling.PrefabFolder + "/" + Ids[i] + ".prefab"));
                    instances.Add(go); go.transform.position = new Vector3(-4.2f + i * 2.8f, 0, 0);
                    go.GetComponent<CombatantView>().nameLabel.gameObject.SetActive(false);
                }
                camera.transform.position = new Vector3(2.5f, 3.2f, 12); camera.transform.LookAt(new Vector3(0, 1.4f, 0)); camera.orthographicSize = 2.25f;
                Capture(camera, 1800, 720, Evidence + "/character-lineup-front.png");
                foreach (var go in instances) go.transform.rotation = Quaternion.Euler(0, 180, 0);
                Capture(camera, 1800, 720, Evidence + "/character-lineup-back.png");
                foreach (var go in instances) Object.DestroyImmediate(go);
                for (int i = 0; i < Ids.Length; i++)
                {
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterModeling.PrefabFolder + "/" + Ids[i] + ".prefab"));
                    go.GetComponent<CombatantView>().nameLabel.gameObject.SetActive(false);
                    var rig = go.GetComponentInChildren<CharacterRig>();
                    camera.orthographicSize = 2.2f; camera.transform.position = new Vector3(4.5f, 2.8f, 10); camera.transform.LookAt(new Vector3(0, 1.5f, 0));
                    var sheet = new Texture2D(480 * 6, 640, TextureFormat.RGB24, false);
                    for (int pose = 0; pose < 6; pose++)
                    {
                        ApplyPose(rig, pose);
                        var bounds = BoundsFor(rig);
                        var center = rig.transform.TransformPoint(bounds.center);
                        camera.transform.position = center + new Vector3(4.5f, 2.4f, 10);
                        camera.transform.LookAt(center);
                        camera.orthographicSize = Mathf.Max(1.85f, bounds.size.y * .64f, bounds.size.x * .85f);
                        FitPose(camera, rig, 480f / 640f);
                        var image = Render(camera, 480, 640);
                        sheet.SetPixels(pose * 480, 0, 480, 640, image.GetPixels()); Object.DestroyImmediate(image);
                    }
                    sheet.Apply(); File.WriteAllBytes(Evidence + "/" + Ids[i] + "-pose-sheet.png", sheet.EncodeToPNG()); Object.DestroyImmediate(sheet);
                    rig.RestoreRestPose(); Object.DestroyImmediate(go);
                }
            }
            finally { EditorSceneManager.OpenScene(original, OpenSceneMode.Single); }
        }
        private static void Capture(Camera camera, int width, int height, string path)
        { var image = Render(camera, width, height); File.WriteAllBytes(path, image.EncodeToPNG()); Object.DestroyImmediate(image); }
        private static void FitPose(Camera camera, CharacterRig rig, float aspect)
        {
            var points = rig.GetComponentsInChildren<MeshFilter>().SelectMany(f => f.sharedMesh.vertices.Select(v => camera.transform.InverseTransformPoint(f.transform.TransformPoint(v)))).ToArray();
            float minX = points.Min(p => p.x), maxX = points.Max(p => p.x), minY = points.Min(p => p.y), maxY = points.Max(p => p.y);
            camera.transform.position += camera.transform.right * ((minX + maxX) * .5f) + camera.transform.up * ((minY + maxY) * .5f);
            camera.orthographicSize = Mathf.Max(1.85f, (maxY - minY) * .57f, (maxX - minX) / aspect * .57f);
        }
        private static Texture2D Render(Camera camera, int width, int height)
        {
            var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active; float oldAspect = camera.aspect;
            var rt = RenderTexture.GetTemporary(width, height, 24);
            try
            {
                camera.targetTexture = rt; camera.aspect = (float)width / height; camera.Render(); RenderTexture.active = rt;
                var result = new Texture2D(width, height, TextureFormat.RGB24, false); result.ReadPixels(new Rect(0, 0, width, height), 0, 0); result.Apply(); return result;
            }
            finally { camera.targetTexture = oldTarget; camera.aspect = oldAspect; RenderTexture.active = oldActive; RenderTexture.ReleaseTemporary(rt); }
        }
    }
}
