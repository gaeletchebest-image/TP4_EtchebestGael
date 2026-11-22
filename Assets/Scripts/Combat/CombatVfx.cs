using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltimaGuardia
{
    // Transient, action-owned effects. The presentation clock owns their lifetime.
    public sealed class CombatVfx : IDisposable
    {
        private readonly GameObject root;
        private readonly Material material;
        private readonly Material particleMaterial;
        private readonly Texture2D glowTexture;
        private readonly List<ParticleSystem> particles = new List<ParticleSystem>();
        private readonly List<LineRenderer> lines = new List<LineRenderer>();
        private readonly List<Transform> targets = new List<Transform>();
        private readonly Transform caster;
        private bool storm, paused;

        public CombatVfx(CombatantView actor, IEnumerable<CombatantView> affected, bool special)
        {
            root = new GameObject("H10_ActionVfx");
            caster = actor.effectAnchor;
            foreach (var target in affected) targets.Add(target.effectAnchor);
            storm = special;
            // The prefab reference keeps this shader in the player build.
            // Shader.Find alone can succeed in the editor and be stripped in a build.
            material = new Material(actor.projectileMaterial);
            material.SetColor("_BaseColor", new Color(.25f, .72f, 1f));
            material.SetColor("_Color", new Color(.25f, .72f, 1f));
            particleMaterial = new Material(actor.projectileMaterial);
            particleMaterial.SetFloat("_Surface", 1); particleMaterial.SetFloat("_SrcBlend", 5);
            particleMaterial.SetFloat("_DstBlend", 10); particleMaterial.SetFloat("_ZWrite", 0);
            particleMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); particleMaterial.renderQueue = 3000;
            glowTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            for (int y = 0; y < 32; y++) for (int x = 0; x < 32; x++)
            {
                float r = Vector2.Distance(new Vector2(x, y), new Vector2(15.5f, 15.5f)) / 15.5f;
                glowTexture.SetPixel(x, y, new Color(1, 1, 1, Mathf.Pow(Mathf.Clamp01(1 - r), 2)));
            }
            glowTexture.Apply(); particleMaterial.SetTexture("_BaseMap", glowTexture);
            if (storm)
            {
                for (int i = 0; i < 2; i++) lines.Add(Line("ArcaneCircle" + i, 64, new Color(.12f, .65f, 1f, 1f), .065f));
                Burst(caster.position, new Color(.15f, .5f, 1f), 0, 60, 1.4f, .16f);
            }
        }

        private LineRenderer Line(string name, int count, Color color, float width)
        {
            var go = new GameObject(name); go.transform.SetParent(root.transform);
            var line = go.AddComponent<LineRenderer>(); line.sharedMaterial = material;
            line.useWorldSpace = true; line.positionCount = count; line.widthMultiplier = width;
            line.startColor = line.endColor = color; line.numCapVertices = 3;
            return line;
        }

        public void Tick(float time, bool isPaused)
        {
            if (paused != isPaused)
            {
                paused = isPaused;
                foreach (var particle in particles)
                    if (isPaused) particle.Pause(true); else particle.Play(true);
            }
            if (!storm || isPaused) return;
            float grow = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.4f, 1.15f, time));
            for (int ring = 0; ring < 2; ring++)
            {
                var line = lines[ring];
                float radius = (.55f + ring * .32f) * grow;
                for (int i = 0; i < line.positionCount; i++)
                {
                    float angle = i * Mathf.PI * 2 / (line.positionCount - 1) + time * (ring == 0 ? 1 : -1);
                    line.SetPosition(i, caster.position + new Vector3(Mathf.Cos(angle) * radius, .08f + ring * .16f, Mathf.Sin(angle) * radius));
                }
                line.enabled = time >= .4f && time < 1.3f;
            }
            for (int i = 2; i < lines.Count; i++) lines[i].enabled = time < 1.48f;
        }

        public void Impact(PendingCombatAction action, CombatantView[] views)
        {
            Color color = action.Kind == CombatAction.Poison ? new Color(.32f, .9f, .22f) :
                action.Kind == CombatAction.CorrosiveFlask ? new Color(1f, .42f, .15f) :
                action.Kind == CombatAction.HasteTonic || action.Kind == CombatAction.Ether ? new Color(.18f, .76f, 1f) :
                action.Kind == CombatAction.Heal || action.Kind == CombatAction.DarkHeal || action.Kind == CombatAction.Potion || action.Kind == CombatAction.Antidote ? new Color(.35f, 1f, .64f) :
                action.Kind == CombatAction.HeavyStrike || action.Kind == CombatAction.Cleave || action.Actor.Definition.Id == "warrior" || action.Actor.Definition.Id == "ogre" ? new Color(1f, .72f, .28f) : new Color(.3f, .62f, 1f);
            foreach (var hit in action.Impacts)
            {
                CombatantView view = Array.Find(views, v => v.combatantId == hit.Target.Definition.Id);
                if (view == null) continue;
                var point = view.effectAnchor.position;
                Burst(point, color, 18, storm ? 42 : 15, storm ? .7f : .38f, storm ? .14f : .08f);
                if (storm)
                {
                    var ray = Line("ArcaneRay", 2, new Color(.35f, .8f, 1f), .11f);
                    ray.SetPosition(0, point + Vector3.up * 4.5f);
                    ray.SetPosition(1, point);
                    lines.Add(ray);
                }
            }
        }

        public void PoisonPulse() => Burst(caster.position, new Color(.32f, .9f, .22f), 12, 12, .45f, .09f);

        private void Burst(Vector3 point, Color color, int burst, int count, float lifetime, float size)
        {
            var go = new GameObject("Particles"); go.transform.SetParent(root.transform); go.transform.position = point;
            var ps = go.AddComponent<ParticleSystem>(); ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main; main.playOnAwake = false; main.loop = burst == 0; main.duration = 2.5f;
            main.startLifetime = lifetime; main.startSpeed = burst == 0 ? .7f : 1.8f;
            main.startSize = size; main.startColor = color; main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = ps.emission; emission.rateOverTime = burst == 0 ? count : 0;
            if (burst > 0) emission.SetBursts(new[] { new ParticleSystem.Burst(0, (short)count) });
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = .12f;
            var renderer = ps.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial = particleMaterial;
            particles.Add(ps); ps.Play(true);
            if (paused) ps.Pause(true);
        }

        public void Dispose()
        {
            if (root != null) { root.SetActive(false); Object.Destroy(root); }
            if (material != null) Object.Destroy(material);
            if (particleMaterial != null) Object.Destroy(particleMaterial);
            if (glowTexture != null) Object.Destroy(glowTexture);
        }
    }
}
