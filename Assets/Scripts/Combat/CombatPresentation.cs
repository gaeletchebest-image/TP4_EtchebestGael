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
        private CombatVfx vfx;
        private Vector3 launchPosition;
        private bool impactSent;
        private float duration;
        public float Time { get; private set; }
        public float ImpactTime { get; private set; }
        public bool HasProjectile => projectile != null;
        public bool IsStorm => pending?.Kind == CombatAction.ArcaneStorm;
        public void SetPaused(bool paused) { if (pending != null) vfx?.Tick(Time, paused); }
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
            vfx = new CombatVfx(actor, views.Where(v => pending.Targets.Any(t => t.Definition.Id == v.combatantId)), IsStorm);
            GameAudio.Current.Started(pending);
        }

        public bool Tick(float seconds)
        {
            if (pending == null) return false;
            vfx?.Tick(Time, session.IsPaused);
            if (session.IsPaused || seconds <= 0) return false;
            Time += seconds;
            vfx?.Tick(Time, false);
            actor.GetComponentInChildren<CharacterAnimationPlayer>()?.SynchronizeAction(Time);
            if (binding != null && binding.projectile && Time >= binding.launchTime && Time < ImpactTime)
            {
                if (projectile == null)
                {
                    launchPosition = actor.projectileOrigin.position;
                    projectile = Object.Instantiate(actor.projectilePrefab);
                    projectile.name = "H06_ConfirmedProjectile";
                    projectile.GetComponent<Renderer>().sharedMaterial = actor.projectileMaterial;
                    if (pending.Actor.Definition.Id == "shaman") projectile.transform.localScale *= .8f;
                    else if (pending.Actor.Definition.Id == "mage") projectile.transform.localScale *= 1.1f;
                }
                float flight = Mathf.InverseLerp(binding.launchTime, ImpactTime, Time);
                projectile.transform.position = Vector3.Lerp(launchPosition, target.effectAnchor.position, flight)
                    + Vector3.up * (Mathf.Sin(flight * Mathf.PI) * .25f);
            }
            if (!impactSent && Time >= ImpactTime)
            {
                impactSent = true;
                if (session.ApplyImpact()) { vfx?.Impact(pending, views); GameAudio.Current.Impact(pending); }
                RemoveProjectile();
            }
            if (Time < duration) return false;
            actor.GetComponentInChildren<CharacterAnimationPlayer>()?.EndAction();
            vfx?.Dispose(); vfx = null;
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
            vfx?.Dispose(); vfx = null;
            if (actor != null) actor.GetComponentInChildren<CharacterAnimationPlayer>()?.EndAction();
            pending = null; session = null; binding = null; actor = target = null;
            Time = 0; impactSent = false;
        }
        public void Dispose() => Cancel();
    }
}
