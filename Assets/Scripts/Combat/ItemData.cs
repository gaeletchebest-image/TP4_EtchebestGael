using UnityEngine;

namespace UltimaGuardia
{
    public enum ItemEffect { Heal, RestoreMana, ApplyStatus, RemovePoison }

    [CreateAssetMenu(menuName = "Ultima Guardia/Objeto")]
    public sealed class ItemData : ScriptableObject
    {
        public string id, displayName;
        public CombatAction command;
        [TextArea] public string description;
        public Sprite icon;
        public TargetMode targetMode;
        public ItemEffect effect;
        public int amount;
        public float recovery = 0.8f;
        public EffectData appliedEffect;
    }
}
