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
    // Editable animation source. Samples rigid-joint poses into ordinary .anim assets.
    // Explicit command only: never rebuilds H05 meshes, scenes, statistics or inventory.
    public static class CharacterAnimationAuthoring
    {
        public const string Folder = "Assets/Animations/H06";
        public const string Evidence = "Documentation/Evidence/H06";
        public static readonly string[] Ids = { "warrior", "mage", "ogre", "shaman" };

        [MenuItem("Ultima Guardia/H06/Crear clips y conectar animaciones")]
        public static void BuildAndIntegrate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty ||
                EditorSceneManager.GetActiveScene().path != "Assets/Scenes/Combat.unity")
                throw new InvalidOperationException("Abrir Combat, guardar y detener Play antes de integrar H06.");
            Directory.CreateDirectory(Folder); Directory.CreateDirectory(Evidence);
            AssetDatabase.Refresh();
            var projectile = CreateProp("Projectile", .105f, .14f);
            var flask = CreateProp("Item", .065f, .15f);
            var itemMaterial = Material("Item", new Color(.9f, .67f, .27f));
            foreach (var id in Ids)
            {
                string prefabPath = CharacterModeling.PrefabFolder + "/" + id + ".prefab";
                var root = PrefabUtility.LoadPrefabContents(prefabPath);
                try
                {
                    var rig = root.GetComponentInChildren<CharacterRig>();
                    var animator = rig.GetComponent<Animator>();
                    if (animator == null) animator = rig.gameObject.AddComponent<Animator>();
                    var player = rig.GetComponent<CharacterAnimationPlayer>();
                    if (player == null) player = rig.gameObject.AddComponent<CharacterAnimationPlayer>();
                    var set = AssetDatabase.LoadAssetAtPath<CharacterAnimationSet>(Folder + "/" + id + ".asset");
                    if (set == null) { set = ScriptableObject.CreateInstance<CharacterAnimationSet>(); AssetDatabase.CreateAsset(set, Folder + "/" + id + ".asset"); }
                    set.idle = Clip(rig, id, "Idle", 2.4f);
                    set.reaction = Clip(rig, id, "Reaction", .38f);
                    set.death = Clip(rig, id, "Death", .45f);
                    set.item = id == "warrior" || id == "mage" ? Clip(rig, id, "Item", .85f) : null;
                    var commands = rig.definition.actions;
                    set.actions = commands.Select(data => new ActionAnimation {
                        command = data.command,
                        clip = Clip(rig, id, data.command.ToString(), data.command == CombatAction.ArcaneStorm ? 2.5f :
                            data.command == CombatAction.HeavyStrike || data.command == CombatAction.Cleave ? 1f :
                            id == "mage" || id == "shaman" ? .95f : .85f),
                        impactTime = data.command == CombatAction.ArcaneStorm ? 1.2f : .35f,
                        launchTime = .15f,
                        projectile = data.command == CombatAction.Basic && (id == "mage" || id == "shaman"),
                        projectileColor = id == "mage" ? Color.cyan : new Color(.6f, .3f, .95f)
                    }).ToArray();
                    foreach (var data in commands)
                    { data.animationClip = set.Find(data.command).clip; EditorUtility.SetDirty(data); }
                    var controller = Controller(id, set);
                    animator.runtimeAnimatorController = controller;
                    animator.applyRootMotion = false; animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    player.animations = set;
                    if (set.item != null)
                    {
                        var prop = rig.itemOrigin.Find("H06_ItemProp");
                        if (prop == null)
                        {
                            var item = (GameObject)PrefabUtility.InstantiatePrefab(flask);
                            item.name = "H06_ItemProp"; item.transform.SetParent(rig.itemOrigin, false); prop = item.transform;
                        }
                        prop.GetComponent<Renderer>().sharedMaterial = itemMaterial;
                        player.itemProp = prop.gameObject; player.itemProp.SetActive(false);
                    }
                    var view = root.GetComponent<CombatantView>();
                    view.projectilePrefab = projectile;
                    view.projectileMaterial = Material(id + "Projectile", id == "mage" ? new Color(.15f, .8f, 1f) : new Color(.75f, .25f, 1f));
                    rig.RestoreRestPose(); EditorUtility.SetDirty(set);
                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
            // Prefab changes propagate without replacing instances or their scene bindings.
            PlayerSettings.bundleVersion = "0.0.7";
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("H06: clips, cuatro controladores, objetos y proyectiles integrados; reglas conservadas.");
        }

        private static AnimatorController Controller(string id, CharacterAnimationSet set)
        {
            string path = Folder + "/" + id + ".controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            var machine = controller.layers[0].stateMachine;
            foreach (var child in machine.states) machine.RemoveState(child.state);
            foreach (var transition in machine.anyStateTransitions) machine.RemoveAnyStateTransition(transition);
            controller.parameters = Array.Empty<AnimatorControllerParameter>();
            controller.AddParameter("Dead", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Executing", AnimatorControllerParameterType.Bool);
            var clips = new Dictionary<string, AnimationClip> { ["Idle"] = set.idle, ["Reaction"] = set.reaction, ["Death"] = set.death };
            if (set.item != null) clips.Add("Item", set.item);
            foreach (var action in set.actions) clips.Add(action.command.ToString(), action.clip);
            var states = new Dictionary<string, AnimatorState>();
            int row = 0;
            foreach (var pair in clips)
            {
                var state = machine.AddState(pair.Key, new Vector3(240, row++ * 75)); state.motion = pair.Value;
                state.writeDefaultValues = true; states.Add(pair.Key, state);
            }
            machine.defaultState = states["Idle"];
            var death = machine.AddAnyStateTransition(states["Death"]);
            death.hasExitTime = false; death.duration = 0; death.canTransitionToSelf = false;
            death.AddCondition(AnimatorConditionMode.If, 0, "Dead");
            foreach (var state in states.Where(p => p.Key != "Idle" && p.Key != "Death"))
            {
                var back = state.Value.AddTransition(states["Idle"]); back.hasExitTime = false; back.duration = 0;
                back.AddCondition(AnimatorConditionMode.IfNot, 0, "Executing");
                back.AddCondition(AnimatorConditionMode.IfNot, 0, "Dead");
            }
            EditorUtility.SetDirty(controller); return controller;
        }

        private static AnimationClip Clip(CharacterRig rig, string id, string state, float duration)
        {
            string path = Folder + "/" + id + "_" + state + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, path); }
            clip.ClearCurves(); clip.name = id + "_" + state; clip.frameRate = 30;
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = state == "Idle"; settings.loopBlend = state == "Idle";
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            AnimationUtility.SetAnimationEvents(clip, Array.Empty<AnimationEvent>());
            int count = Mathf.CeilToInt(duration * 30) + 1;
            var positions = new Vector3[rig.joints.Length, count]; var rotations = new Quaternion[rig.joints.Length, count];
            for (int frame = 0; frame < count; frame++)
            {
                float time = duration * frame / (count - 1);
                Pose(rig, state, time, duration);
                for (int j = 0; j < rig.joints.Length; j++)
                { positions[j, frame] = rig.joints[j].localPosition; rotations[j, frame] = rig.joints[j].localRotation; }
            }
            for (int j = 0; j < rig.joints.Length; j++)
            {
                string jointPath = AnimationUtility.CalculateTransformPath(rig.joints[j], rig.transform);
                for (int axis = 0; axis < 3; axis++)
                {
                    var keys = new Keyframe[count];
                    for (int f = 0; f < count; f++) keys[f] = new Keyframe(duration * f / (count - 1), positions[j, f][axis]);
                    Curve(clip, jointPath, "m_LocalPosition." + "xyz"[axis], keys);
                }
                for (int axis = 0; axis < 4; axis++)
                {
                    var keys = new Keyframe[count];
                    for (int f = 0; f < count; f++) keys[f] = new Keyframe(duration * f / (count - 1), rotations[j, f][axis]);
                    Curve(clip, jointPath, "m_LocalRotation." + "xyzw"[axis], keys);
                }
            }
            clip.EnsureQuaternionContinuity(); rig.RestoreRestPose(); EditorUtility.SetDirty(clip); return clip;
        }

        private static void Curve(AnimationClip clip, string path, string property, Keyframe[] keys)
        {
            var curve = new AnimationCurve(keys);
            for (int k = 0; k < keys.Length; k++)
            { AnimationUtility.SetKeyLeftTangentMode(curve, k, AnimationUtility.TangentMode.Linear); AnimationUtility.SetKeyRightTangentMode(curve, k, AnimationUtility.TangentMode.Linear); }
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, typeof(Transform), property), curve);
        }

        public static void Pose(CharacterRig rig, string state, float time, float duration)
        {
            rig.RestoreRestPose();
            bool ogre = rig.definition.id == "ogre", physical = ogre || rig.definition.id == "warrior";
            float u = Mathf.Clamp01(time / duration);
            if (state == "Idle")
            {
                float breath = Mathf.Sin(u * Mathf.PI * 2);
                Offset(rig, rig.chest, new Vector3(1.3f * breath, .7f * breath, 0));
                Offset(rig, rig.head, new Vector3(-.7f * breath, 0, 0));
                Offset(rig, rig.leftUpperArm, new Vector3(.8f * breath, 0, .8f * breath)); return;
            }
            if (state == "Death")
            {
                float fall = Mathf.SmoothStep(0, 1, u);
                rig.hips.localPosition = Vector3.Lerp(rig.hips.localPosition, new Vector3(0, .45f, .05f), fall);
                Offset(rig, rig.hips, new Vector3(0, 0, 85 * fall));
                Offset(rig, rig.chest, new Vector3(0, 0, -5 * fall));
                Offset(rig, rig.rightUpperArm, new Vector3(-10, 0, -50) * fall);
                Offset(rig, rig.leftUpperArm, new Vector3(-70, 0, 80) * fall);
                Offset(rig, rig.leftForearm, new Vector3(-30, 0, 0) * fall);
                Offset(rig, rig.rightForearm, new Vector3(-60, 0, 0) * fall);
                Offset(rig, rig.rightThigh, new Vector3(-20, 0, -12) * fall);
                Offset(rig, rig.rightShin, new Vector3(45, 0, 0) * fall);
                // Ground the actual authored mesh (including staff, robes and ogre weapon).
                float floor = CharacterArtValidation.BoundsFor(rig).min.y;
                if (floor < 0) rig.hips.localPosition += Vector3.up * -floor;
                return;
            }
            float gesture = Envelope(time, .2f, .35f, duration);
            if (state == "Reaction")
            {
                gesture = Mathf.Sin(Mathf.PI * u);
                Offset(rig, rig.chest, new Vector3(-20, 0, 8) * gesture);
                Offset(rig, rig.head, new Vector3(-12, 10, 0) * gesture);
                Offset(rig, rig.leftUpperArm, new Vector3(-25, 0, 25) * gesture); return;
            }
            if (state == "Item")
            {
                Offset(rig, rig.leftUpperArm, new Vector3(-65, 0, 15) * gesture);
                Offset(rig, rig.leftForearm, new Vector3(-60, 0, 0) * gesture);
                Offset(rig, rig.head, new Vector3(8, -15, 0) * gesture); return;
            }
            if (physical)
            {
                bool heavy = state == "HeavyStrike" || state == "Cleave";
                float windup = Envelope(time, .19f, .24f, .36f);
                float strike = Envelope(time, .35f, .43f, duration);
                Offset(rig, rig.chest, new Vector3((heavy ? -15 : -7) * windup + 17 * strike,
                    -(heavy ? 36 : 22) * windup + (heavy ? 30 : 20) * strike, 0));
                Offset(rig, rig.rightUpperArm, new Vector3((heavy ? -145 : -90) * windup - 35 * strike, 0,
                    -25 * windup + 28 * strike));
                Offset(rig, rig.rightForearm, new Vector3(-40 * windup - 20 * strike, 0, 0));
                Offset(rig, rig.leftUpperArm, new Vector3(-25, 0, 22) * gesture);
                rig.hips.localPosition += new Vector3(0, 0, (ogre ? .22f : .18f) * gesture);
                Offset(rig, rig.rightThigh, new Vector3(-12, 0, 0) * gesture);
                Offset(rig, rig.rightShin, new Vector3(17, 0, 0) * gesture);
                // H07 contact correction: cancel the thigh/shin net pitch at the ankle.
                // The step still lifts the foot, but its toe cannot sink into the paving.
                Offset(rig, rig.rightFoot, new Vector3(-5, 0, 0) * gesture);
            }
            else
            {
                bool storm = state == "ArcaneStorm";
                gesture = storm ? Envelope(time, .4f, 1.2f, 2.5f) : gesture;
                bool heal = state == "Heal" || state == "DarkHeal";
                Offset(rig, rig.rightUpperArm, new Vector3(storm ? -145 : heal ? -90 : -68, 0, -12) * gesture);
                Offset(rig, rig.rightForearm, new Vector3(-20, 0, 0) * gesture);
                Offset(rig, rig.leftUpperArm, new Vector3(heal ? -75 : -45, 0, 25) * gesture);
                Offset(rig, rig.leftForearm, new Vector3(-25, 0, 0) * gesture);
                Offset(rig, rig.head, new Vector3(storm ? -12 : 4, 0, 0) * gesture);
                Offset(rig, rig.chest, new Vector3(-5, state == "Poison" ? -10 : 0, 0) * gesture);
                // Keep the staff upright as the shoulder rises; keyed on the wrist.
                Quaternion wrist = Quaternion.Inverse(rig.rightHand.parent.rotation) * rig.transform.rotation * Quaternion.Euler(storm ? 8 : 20, 0, -8);
                rig.rightHand.localRotation = Quaternion.Slerp(rig.rightHand.localRotation, wrist, gesture);
            }
        }
        // Targeted migration: preserve controllers, prefabs, data and all other H06 clips.
        public static void CorrectPhysicalFootContact()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Detener Play antes de corregir los clips.");
            foreach (string id in new[] { "warrior", "ogre" })
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CharacterModeling.PrefabFolder + "/" + id + ".prefab");
                var copy = Object.Instantiate(prefab);
                try
                {
                    var rig = copy.GetComponentInChildren<CharacterRig>();
                    rig.GetComponent<Animator>().enabled = false;
                    foreach (var action in rig.GetComponent<CharacterAnimationPlayer>().animations.actions)
                        Clip(rig, id, action.command.ToString(), action.clip.length);
                }
                finally { Object.DestroyImmediate(copy); }
            }
            AssetDatabase.SaveAssets();
        }
        private static float Envelope(float t, float peak, float hold, float end)
            => t < peak ? Mathf.SmoothStep(0, 1, t / peak) : t <= hold ? 1 : 1 - Mathf.SmoothStep(0, 1, Mathf.InverseLerp(hold, end, t));
        private static void Offset(CharacterRig rig, Transform joint, Vector3 euler)
        { int index = Array.IndexOf(rig.joints, joint); joint.localRotation = rig.restRotations[index] * Quaternion.Euler(euler); }

        private static Material Material(string name, Color color)
        {
            string path = Folder + "/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(material, path); }
            material.color = color; EditorUtility.SetDirty(material); return material;
        }
        private static GameObject CreateProp(string name, float radius, float height)
        {
            var mesh = new Mesh { name = name };
            mesh.vertices = new[] { Vector3.up * height, Vector3.right * radius, Vector3.forward * radius, Vector3.left * radius, Vector3.back * radius, Vector3.down * height };
            mesh.triangles = new[] { 0,2,1, 0,3,2, 0,4,3, 0,1,4, 5,1,2, 5,2,3, 5,3,4, 5,4,1 };
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            string path = Folder + "/" + name + ".asset";
            var saved = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (saved == null) { AssetDatabase.CreateAsset(mesh, path); saved = mesh; }
            else { EditorUtility.CopySerialized(mesh, saved); Object.DestroyImmediate(mesh); }
            var go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            go.GetComponent<MeshFilter>().sharedMesh = saved;
            go.GetComponent<MeshRenderer>().sharedMaterial = Material(name, Color.white);
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, Folder + "/" + name + ".prefab"); Object.DestroyImmediate(go); return prefab;
        }
    }
}
