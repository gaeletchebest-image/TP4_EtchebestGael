using UnityEngine;

namespace UltimaGuardia
{
    public enum ActionEffect { Damage, Heal, RestoreMana, ApplyStatus }

    [CreateAssetMenu(menuName = "Ultima Guardia/Acción")]
    public sealed class ActionData : ScriptableObject
    {
        public string id, displayName;
        [TextArea] public string description;
        public CombatAction command;
        public ActionEffect effect;
        public int manaCost, fixedAmount;
        public float potency = 1, recovery = 0.6f;
        public TargetMode targetMode;
        public EffectData appliedEffect;
        public bool specialPresentation;
        public AnimationClip animationClip;
        public Sprite icon;
        public AudioClip audioClip;
        public GameObject vfxPrefab;
    }
}
