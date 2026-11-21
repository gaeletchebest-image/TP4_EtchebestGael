using System;
using UnityEngine;

namespace UltimaGuardia
{
    [Serializable]
    public sealed class ActionAnimation
    {
        public CombatAction command;
        public AnimationClip clip;
        public float impactTime = .35f;
        public float launchTime = .15f;
        public bool projectile;
        public Color projectileColor = Color.cyan;
    }

    [CreateAssetMenu(menuName = "Ultima Guardia/Animaciones de personaje")]
    public sealed class CharacterAnimationSet : ScriptableObject
    {
        public AnimationClip idle, reaction, death, item;
        public float itemImpactTime = .35f;
        public ActionAnimation[] actions = Array.Empty<ActionAnimation>();
        public ActionAnimation Find(CombatAction command) => Array.Find(actions, a => a.command == command);
    }
}
