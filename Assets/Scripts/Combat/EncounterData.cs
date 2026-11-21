using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimaGuardia
{
    [Serializable]
    public sealed class EncounterSlot
    {
        public CombatantData combatant;
        public Vector3 position, rotation;
    }

    [Serializable]
    public sealed class InitialItem
    {
        public ItemData item;
        public int quantity;
    }

    [CreateAssetMenu(menuName = "Ultima Guardia/Encuentro")]
    public sealed class EncounterData : ScriptableObject
    {
        public string id = "main_encounter";
        public EncounterSlot[] participants = new EncounterSlot[0];
        public InitialItem[] inventory = new InitialItem[0];

        public List<string> ValidateData()
        {
            var errors = new List<string>();
            var ids = new Dictionary<string, UnityEngine.Object>();
            Action<string, UnityEngine.Object> unique = (key, asset) =>
            {
                if (string.IsNullOrWhiteSpace(key)) { errors.Add(asset.name + ": falta identificador."); return; }
                if (ids.TryGetValue(key, out var other) && other != asset)
                    errors.Add(key + ": identificador duplicado en " + other.name + " y " + asset.name + ".");
                else ids[key] = asset;
            };
            var visited = new HashSet<UnityEngine.Object>();
            Action<EffectData> effect = data =>
            {
                if (data == null) { errors.Add("Falta referencia de efecto."); return; }
                if (!visited.Add(data)) return;
                unique(data.id, data);
                if (data.duration <= 0 || data.fixedDamage < 0 || !Finite(data.multiplier) || data.multiplier <= 0)
                    errors.Add(data.name + ": duración, daño o multiplicador de efecto inválido.");
                if (!Enum.IsDefined(typeof(StatusKind), data.kind) || !Enum.IsDefined(typeof(EffectTrigger), data.trigger)) errors.Add(data.name + ": tipo o activación de efecto inválidos.");
            };
            unique(id, this);
            if (participants == null || participants.Length != 4)
                errors.Add(name + ": el encuentro requiere exactamente cuatro participantes.");
            var members = new HashSet<string>();
            int allies = 0, enemies = 0;
            foreach (var slot in participants ?? new EncounterSlot[0])
            {
                if (slot == null || slot.combatant == null) { errors.Add("Falta referencia de combatiente."); continue; }
                var c = slot.combatant;
                unique(c.id, c);
                if (!members.Add(c.id ?? "")) errors.Add(c.name + ": participante duplicado en el encuentro.");
                if (c.team == CombatTeam.Allies) allies++; else if (c.team == CombatTeam.Enemies) enemies++;
                if (c.prefab == null) errors.Add(c.name + ": falta prefab.");
                else if (c.prefab.GetComponent<CombatantView>() == null || c.prefab.GetComponent<CombatantView>().combatantId != c.id)
                    errors.Add(c.name + ": prefab sin identidad de CombatantView compatible.");
                if (string.IsNullOrWhiteSpace(c.displayName) || c.maxHealth <= 0 || c.maxMana < 0 || c.attack < 0 || c.defense < 0 || !Finite(c.speed) || c.speed <= 0)
                    errors.Add(c.name + ": nombre o estadísticas inválidas (velocidad debe ser positiva y finita).");
                if (!Finite(slot.position) || !Finite(slot.rotation)) errors.Add(c.name + ": posición o rotación inválida.");
                if (!Enum.IsDefined(typeof(CombatTeam), c.team) || !Enum.IsDefined(typeof(EnemyBrain), c.brain)) errors.Add(c.name + ": equipo o IA inválidos.");
                if (c.actions == null || c.actions.Length == 0) errors.Add(c.name + ": faltan acciones.");
                var commands = new HashSet<CombatAction>();
                foreach (var a in c.actions ?? new ActionData[0])
                {
                    if (a == null) { errors.Add(c.name + ": falta referencia de acción."); continue; }
                    if (!commands.Add(a.command)) errors.Add(c.name + ": comando duplicado " + a.command + ".");
                    unique(a.id, a);
                    if (string.IsNullOrWhiteSpace(a.displayName) || a.manaCost < 0 || a.fixedAmount < 0 || !Finite(a.potency) || a.potency < 0 || !Finite(a.recovery) || a.recovery < 0)
                        errors.Add(a.name + ": nombre, costo, potencia, cantidad o recuperación inválidos.");
                    if (!Enum.IsDefined(typeof(CombatAction), a.command) || !Enum.IsDefined(typeof(ActionEffect), a.effect) || !Enum.IsDefined(typeof(TargetMode), a.targetMode)) errors.Add(a.name + ": tipo de acción u objetivo inválido.");
                    if (CombatSession.IsItem(a.command)) errors.Add(a.name + ": una habilidad no puede usar un comando de objeto.");
                    if (a.effect == ActionEffect.ApplyStatus) effect(a.appliedEffect);
                }
                if (!(c.actions ?? new ActionData[0]).Any(a => a != null && a.command == CombatAction.Basic && a.manaCost == 0 && a.effect == ActionEffect.Damage && a.targetMode == TargetMode.Enemy))
                    errors.Add(c.name + ": requiere un ataque básico gratuito contra un enemigo.");
            }
            if (allies != 2 || enemies != 2) errors.Add("El encuentro requiere dos aliados y dos enemigos.");
            var itemIds = new HashSet<string>();
            var itemCommands = new HashSet<CombatAction>();
            foreach (var entry in inventory ?? new InitialItem[0])
            {
                if (entry == null || entry.item == null) { errors.Add("Falta referencia de objeto del inventario."); continue; }
                var item = entry.item;
                unique(item.id, item);
                if (!itemIds.Add(item.id ?? "")) errors.Add(item.name + ": objeto duplicado en inventario.");
                if (!CombatSession.IsItem(item.command) || !itemCommands.Add(item.command)) errors.Add(item.name + ": comando de objeto inválido o duplicado.");
                if (entry.quantity < 0 || item.amount < 0 || !Finite(item.recovery) || item.recovery < 0 || string.IsNullOrWhiteSpace(item.displayName)) errors.Add(item.name + ": cantidad, nombre o recuperación inválidos.");
                if (!Enum.IsDefined(typeof(ItemEffect), item.effect) || !Enum.IsDefined(typeof(TargetMode), item.targetMode)) errors.Add(item.name + ": efecto u objetivo inválido.");
                if (item.effect == ItemEffect.ApplyStatus || item.effect == ItemEffect.RemovePoison) effect(item.appliedEffect);
            }
            if (!(inventory ?? new InitialItem[0]).Any(e => e != null && e.item != null && e.item.id == "potion" && e.item.effect == ItemEffect.Heal && e.item.targetMode == TargetMode.Ally)) errors.Add("Falta la Poción de curación aliada del prototipo.");
            return errors;
        }

        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        private static bool Finite(Vector3 value) => Finite(value.x) && Finite(value.y) && Finite(value.z);
        private void OnValidate()
        {
            foreach (string error in ValidateData()) Debug.LogWarning("Encuentro " + name + ": " + error, this);
        }
    }
}
