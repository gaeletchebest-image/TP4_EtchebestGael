using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UltimaGuardia.Editor
{
    // Explicit migration of the existing encounter. Never regenerates scenes or resets statistics.
    public static class ActionsSetup
    {
        private static ActionData Action(string id, string label, CombatAction command, ActionEffect effect,
            int cost, float potency, int amount, float recovery, TargetMode target, string description)
        {
            string path = AtbSetup.Root + id + ".asset";
            var data = AssetDatabase.LoadAssetAtPath<ActionData>(path);
            if (data != null) return data;
            data = ScriptableObject.CreateInstance<ActionData>();
            data.id = id; data.displayName = label; data.command = command; data.effect = effect;
            data.manaCost = cost; data.potency = potency; data.fixedAmount = amount;
            data.recovery = recovery; data.targetMode = target; data.description = description;
            data.specialPresentation = command == CombatAction.ArcaneStorm;
            if (effect == ActionEffect.ApplyStatus) data.appliedEffect = AssetDatabase.LoadAssetAtPath<EffectData>(AtbSetup.Root + "poison.asset");
            AssetDatabase.CreateAsset(data, path);
            return data;
        }

        [MenuItem("Ultima Guardia/H03/Integrar acciones e inventario")]
        public static void Install()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Detener Play y guardar la escena antes de integrar H03.");
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>(AtbSetup.Root + "MainEncounter.asset");
            if (encounter == null) throw new InvalidOperationException("Falta el encuentro integrado de H02.");
            var heal = Action("heal", "Curación", CombatAction.Heal, ActionEffect.Heal, 10, 0, 36, 1.2f, TargetMode.Ally, "Recupera vida de un aliado vivo, incluida la maga. Requiere una herida.");
            var storm = Action("arcane_storm", "Tormenta arcana", CombatAction.ArcaneStorm, ActionEffect.Damage, 18, 1.6f, 0, 2.8f, TargetMode.AllEnemies, "Daña a todos los enemigos vivos. Un único costo de maná.");
            var cleave = Action("cleave", "Machetazo", CombatAction.Cleave, ActionEffect.Damage, 8, 1.5f, 0, 2.2f, TargetMode.Enemy, "Golpe potente contra un aliado del jugador vivo.");
            var poison = Action("poison_spell", "Veneno", CombatAction.Poison, ActionEffect.ApplyStatus, 6, 0, 0, 1.2f, TargetMode.Enemy, "Sin daño directo. Veneno: 8 de daño en tres inicios de turno. Pendiente de H04.");
            var darkHeal = Action("dark_heal", "Curación oscura", CombatAction.DarkHeal, ActionEffect.Heal, 10, 0, 40, 1.5f, TargetMode.Ally, "Recupera 40 de vida del ogro vivo. No cura al chamán.");
            foreach (var slot in encounter.participants)
            {
                var fighter = slot.combatant;
                var additions = fighter.id == "mage" ? new[] { heal, storm } : fighter.id == "ogre" ? new[] { cleave } : fighter.id == "shaman" ? new[] { poison, darkHeal } : new ActionData[0];
                fighter.actions = fighter.actions.Concat(additions.Where(a => !fighter.actions.Any(old => old.command == a.command))).ToArray();
                if (fighter.id == "ogre") fighter.brain = EnemyBrain.Ogre;
                if (fighter.id == "shaman") fighter.brain = EnemyBrain.Shaman;
                EditorUtility.SetDirty(fighter);
            }
            foreach (var entry in encounter.inventory)
            {
                var item = entry.item;
                switch (item.id)
                {
                    case "potion": item.command = CombatAction.Potion; item.description = "Recupera 50 de vida de un aliado vivo herido."; break;
                    case "ether": item.command = CombatAction.Ether; item.description = "Recupera 20 de maná de un aliado vivo con maná faltante."; break;
                    case "haste_tonic": item.command = CombatAction.HasteTonic; item.description = "Un aliado vivo: velocidad ×1,5 durante sus próximas 2 acciones. Pendiente de H04."; break;
                    case "corrosive_flask": item.command = CombatAction.CorrosiveFlask; item.description = "Un enemigo vivo: defensa ×0,5 durante sus próximas 2 acciones. Pendiente de H04."; break;
                    case "antidote": item.command = CombatAction.Antidote; item.description = "Elimina Veneno de un aliado vivo afectado. Pendiente de H04."; break;
                }
                EditorUtility.SetDirty(item);
            }
            var errors = encounter.ValidateData();
            if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Combat.unity");
            var canvas = GameObject.Find("CombatCanvas").transform;
            // Five readable item rows fit within the existing decision panel.
            var commands = canvas.Find("Decision/Commands").GetComponent<Text>();
            commands.fontSize = 22;
            commands.rectTransform.sizeDelta = new Vector2(450, 170);
            var description = canvas.Find("Decision/Description").GetComponent<Text>();
            description.fontSize = 20;
            description.rectTransform.sizeDelta = new Vector2(720, 166);
            var events = canvas.Find("LastEvent").GetComponent<Text>();
            events.fontSize = 19;
            PlayerSettings.bundleVersion = "0.0.4";
            EditorUtility.SetDirty(commands); EditorUtility.SetDirty(description); EditorUtility.SetDirty(events);
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            Debug.Log("H03: acciones, inventario e IA integrados. Estados pendientes de H04; presentación especial pendiente de H10.");
        }
    }
}
