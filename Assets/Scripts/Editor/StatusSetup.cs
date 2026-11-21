using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UltimaGuardia.Editor
{
    public static class StatusSetup
    {
        [MenuItem("Ultima Guardia/H04/Integrar estados temporales")]
        public static void Install()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Detener Play y guardar la escena antes de integrar H04.");
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>(AtbSetup.Root + "MainEncounter.asset");
            if (encounter == null) throw new InvalidOperationException("Falta el encuentro de H03.");
            foreach (var entry in encounter.inventory)
            {
                entry.item.description = entry.item.description.Replace(" Pendiente de H04.", "");
                EditorUtility.SetDirty(entry.item);
            }
            foreach (var slot in encounter.participants)
                foreach (var action in slot.combatant.actions)
                    if (action.effect == ActionEffect.ApplyStatus)
                    { action.description = action.description.Replace(" Pendiente de H04.", ""); EditorUtility.SetDirty(action); }
            var errors = encounter.ValidateData();
            if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Combat.unity");
            // Badges have their own opaque backing; keep the arena's original visible area.
            var header = GameObject.Find("CombatCanvas/Header").GetComponent<RectTransform>();
            header.anchoredPosition = new Vector2(0, 275); header.sizeDelta = new Vector2(1280, 170);
            header.GetComponent<UnityEngine.UI.Image>().color = new Color(0.035f, 0.055f, 0.085f, 1);
            EditorUtility.SetDirty(header);
            PlayerSettings.bundleVersion = "0.0.5";
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            Debug.Log("H04 integrado: estados, consumibles, IA y contadores provisionales. Sin cambios de balance.");
        }
    }
}
