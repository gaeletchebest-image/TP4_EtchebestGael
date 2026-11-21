using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltimaGuardia.Editor
{
    public static class CharacterAnimationValidation
    {
        [Serializable] public sealed class Report { public string version; public bool passed; public List<Entry> characters = new List<Entry>(); }
        [Serializable] public sealed class Entry { public string id; public string[] rows; public int clips; public int joints; public float minimumDeathY; }

        [MenuItem("Ultima Guardia/H06/Auditar clips y capturar secuencias")]
        public static void AuditAndCapture()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Guardar y detener Play antes de auditar H06.");
            string original = EditorSceneManager.GetActiveScene().path;
            var report = new Report { version = Application.version, passed = true };
            Directory.CreateDirectory(CharacterAnimationAuthoring.Evidence);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            try
            {
                RenderSettings.ambientLight = new Color(.6f, .64f, .72f);
                var camera = new GameObject("AnimationReviewCamera", typeof(Camera)).GetComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.055f, .075f, .105f);
                camera.orthographic = true; camera.nearClipPlane = .1f; camera.farClipPlane = 50;
                var light = new GameObject("Key", typeof(Light)).GetComponent<Light>();
                light.type = LightType.Directional; light.intensity = 1.6f; light.transform.rotation = Quaternion.Euler(40, -35, 0);
                var fill = new GameObject("Fill", typeof(Light)).GetComponent<Light>();
                fill.type = LightType.Directional; fill.intensity = .65f; fill.transform.rotation = Quaternion.Euler(20, 140, 0);
                foreach (string id in CharacterAnimationAuthoring.Ids)
                {
                    var root = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterModeling.PrefabFolder + "/" + id + ".prefab"));
                    root.GetComponent<CombatantView>().nameLabel.gameObject.SetActive(false);
                    var rig = root.GetComponentInChildren<CharacterRig>();
                    var player = rig.GetComponent<CharacterAnimationPlayer>(); var animator = rig.GetComponent<Animator>();
                    Require(player != null && animator.runtimeAnimatorController != null, id + ": controlador integrado");
                    animator.enabled = false;
                    var set = player.animations;
                    var clips = new List<AnimationClip> { set.idle };
                    clips.AddRange(set.actions.Select(a => a.clip)); if (set.item != null) clips.Add(set.item);
                    clips.Add(set.reaction); clips.Add(set.death);
                    var controller = (AnimatorController)animator.runtimeAnimatorController;
                    var states = controller.layers[0].stateMachine.states.Select(s => s.state).ToArray();
                    Require(states.Single(s => s.name == "Death").transitions.Length == 0, id + ": muerte terminal");
                    Require(!animator.applyRootMotion && rig.joints.Length == 16, id + ": anclaje y rig");
                    float deathY = 100;
                    var sheet = new Texture2D(256 * 8, 300 * clips.Count, TextureFormat.RGB24, false);
                    for (int row = 0; row < clips.Count; row++)
                    {
                        var clip = clips[row];
                        Require(clip != null && clip.length > 0 && AnimationUtility.GetAnimationEvents(clip).Length == 0, id + ": clip sin eventos de juego");
                        Require(AnimationUtility.GetCurveBindings(clip).Length == 16 * 7, id + ": curvas completas");
                        var points = new List<Vector3>();
                        for (int sample = 0; sample < 8; sample++)
                        {
                            clip.SampleAnimation(rig.gameObject, clip.length * sample / 7);
                            foreach (var f in rig.GetComponentsInChildren<MeshFilter>())
                                points.AddRange(f.sharedMesh.vertices.Select(v => f.transform.TransformPoint(v)));
                            if (clip == set.death) deathY = Mathf.Min(deathY, CharacterArtValidation.BoundsFor(rig).min.y);
                        }
                        var bounds = new Bounds(points[0], Vector3.zero); foreach (var p in points) bounds.Encapsulate(p);
                        camera.transform.position = bounds.center + new Vector3(4.5f, 2.4f, 10);
                        camera.transform.LookAt(bounds.center); camera.aspect = 256f / 300;
                        var projected = points.Select(p => camera.transform.InverseTransformPoint(p)).ToArray();
                        float minX = projected.Min(p => p.x), maxX = projected.Max(p => p.x), minY = projected.Min(p => p.y), maxY = projected.Max(p => p.y);
                        camera.transform.position += camera.transform.right * ((minX + maxX) * .5f) + camera.transform.up * ((minY + maxY) * .5f);
                        camera.orthographicSize = Mathf.Max(1.7f, (maxY - minY) * .58f, (maxX - minX) / camera.aspect * .58f);
                        for (int sample = 0; sample < 8; sample++)
                        {
                            clip.SampleAnimation(rig.gameObject, clip.length * sample / 7);
                            var image = Render(camera, 256, 300);
                            sheet.SetPixels(sample * 256, (clips.Count - row - 1) * 300, 256, 300, image.GetPixels()); Object.DestroyImmediate(image);
                        }
                        if (clip != set.death)
                        {
                            clip.SampleAnimation(rig.gameObject, clip.length);
                            for (int j = 0; j < 16; j++)
                            {
                                Require(Vector3.Distance(rig.joints[j].localPosition, rig.restPositions[j]) < .001f, id + ": posición final " + clip.name);
                                Require(Quaternion.Angle(rig.joints[j].localRotation, rig.restRotations[j]) < .05f, id + ": rotación final " + clip.name);
                            }
                        }
                    }
                    Require(deathY >= -.012f, id + ": caída por encima del suelo");
                    foreach (var action in set.actions)
                        Require(action.impactTime > 0 && action.impactTime < action.clip.length && (!action.projectile || action.launchTime < action.impactTime), id + ": tiempos coherentes");
                    sheet.Apply(); File.WriteAllBytes(CharacterAnimationAuthoring.Evidence + "/" + id + "-animation-sheet.png", sheet.EncodeToPNG()); Object.DestroyImmediate(sheet);
                    report.characters.Add(new Entry { id = id, clips = clips.Count, rows = clips.Select(c => c.name).ToArray(), joints = 16, minimumDeathY = deathY });
                    Object.DestroyImmediate(root);
                }
                File.WriteAllText(CharacterAnimationAuthoring.Evidence + "/animation-audit.json", JsonUtility.ToJson(report, true));
            }
            finally { EditorSceneManager.OpenScene(original, OpenSceneMode.Single); }
        }
        private static void Require(bool test, string message) { if (!test) throw new InvalidOperationException("H06: " + message); }
        private static Texture2D Render(Camera camera, int width, int height)
        {
            var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(width, height, 24);
            try
            {
                camera.targetTexture = rt; camera.Render(); RenderTexture.active = rt;
                var image = new Texture2D(width, height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply(); return image;
            }
            finally { camera.targetTexture = oldTarget; RenderTexture.active = oldActive; RenderTexture.ReleaseTemporary(rt); }
        }
    }
}
