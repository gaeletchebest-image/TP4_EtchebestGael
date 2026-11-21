using UnityEngine;

namespace UltimaGuardia
{
    public enum StatusKind { Poison, Haste, Corrosion }
    public enum EffectTrigger { TurnStart, ActionEnd }

    [CreateAssetMenu(menuName = "Ultima Guardia/Efecto")]
    public sealed class EffectData : ScriptableObject
    {
        public string id, displayName;
        public StatusKind kind;
        public EffectTrigger trigger;
        public int duration = 2, fixedDamage;
        public float multiplier = 1;
        public Sprite icon;
    }
}
