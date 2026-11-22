using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltimaGuardia
{
    // One owner for a committed action, its confirmed destination and its impact signal.
    public sealed class CombatPresentation : IDisposable
    {
        private readonly CombatantView[] views;
        private PendingCombatAction pending;
        private CombatSession session;
        private ActionAnimation binding;
        private CombatantView actor, target;
        private GameObject projectile;
        private Vector3 launchPosition;
        private bool impactSent;
        private float duration;
        public float Time { get; private set; }
        public float ImpactTime { get; private set; }
        public bool HasProjectile => projectile != null;
        public Vector3 ProjectilePosition => projectile != null ? projectile.transform.position : Vector3.zero;
        public string ConfirmedTargetId => pending?.Target.Definition.Id;

        public CombatPresentation(CombatantView[] combatants) { views = combatants; }

        public void Begin(CombatSession combat)
        {
            Cancel(); session = combat; pending = combat.Pending;
            actor = views.First(v => v.combatantId == pending.Actor.Definition.Id);
            target = views.First(v => v.combatantId == pending.Target.Definition.Id);
            var animation = actor.GetComponentInChildren<CharacterAnimationPlayer>();
            binding = animation != null ? animation.animations.Find(pending.Kind) : null;
            ImpactTime = pending.Item != null ? animation?.animations.itemImpactTime ?? .35f : binding?.impactTime ?? .35f;
            duration = pending.Item != null ? animation?.animations.item.length ?? .85f : binding?.clip.length ?? .85f;
            animation?.BeginAction(pending, target.transform.position);
            GameAudio.Current.Started(pending);
        }

        public bool Tick(float seconds)
        {
            if (pending == null || session.IsPaused || seconds <= 0) return false;
            Time += seconds;
            actor.GetComponentInChildren<CharacterAnimationPlayer>()?.SynchronizeAction(Time);
            if (binding != null && binding.projectile && Time >= binding.launchTime && Time < ImpactTime)
            {
                if (projectile == null)
                {
                    launchPosition = actor.projectileOrigin.position;
                    projectile = Object.Instantiate(actor.projectilePrefab);
                    projectile.name = "H06_ConfirmedProjectile";
                    projectile.GetComponent<Renderer>().sharedMaterial = actor.projectileMaterial;
                }
                float flight = Mathf.InverseLerp(binding.launchTime, ImpactTime, Time);
                projectile.transform.position = Vector3.Lerp(launchPosition, target.effectAnchor.position, flight)
                    + Vector3.up * (Mathf.Sin(flight * Mathf.PI) * .25f);
            }
            if (!impactSent && Time >= ImpactTime)
            {
                impactSent = true;
                if (session.ApplyImpact()) GameAudio.Current.Impact(pending);
                RemoveProjectile();
            }
            if (Time < duration) return false;
            actor.GetComponentInChildren<CharacterAnimationPlayer>()?.EndAction();
            pending = null; return true;
        }

        private void RemoveProjectile()
        {
            if (projectile == null) return;
            projectile.SetActive(false); Object.Destroy(projectile); projectile = null;
        }
        public void Cancel()
        {
            RemoveProjectile();
            if (actor != null) actor.GetComponentInChildren<CharacterAnimationPlayer>()?.EndAction();
            pending = null; session = null; binding = null; actor = target = null;
            Time = 0; impactSent = false;
        }
        public void Dispose() => Cancel();
    }
}
