using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltimaGuardia
{
    // One persistent owner. Combat sources can pause while the interface source stays live.
    public sealed class GameAudio : MonoBehaviour
    {
        private static GameAudio instance;
        private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        private readonly List<AudioSource> combat = new List<AudioSource>();
        private AudioSource music, ui;
        private bool paused;
        public static GameAudio Current
        {
            get
            {
                if (instance != null) return instance;
                var go = new GameObject("GameAudio_H09");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<GameAudio>();
                return instance;
            }
        }
        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            music = gameObject.AddComponent<AudioSource>(); music.loop = true; music.playOnAwake = false; music.volume = .32f;
            ui = gameObject.AddComponent<AudioSource>(); ui.playOnAwake = false; ui.volume = .55f;
            for (int i = 0; i < 6; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false; source.volume = .65f; combat.Add(source);
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDestroy()
        {
            if (instance != this) return;
            SceneManager.sceneLoaded -= OnSceneLoaded; instance = null;
        }
        private AudioClip Clip(string id)
        {
            if (!clips.TryGetValue(id, out var clip)) clips[id] = clip = Resources.Load<AudioClip>("Audio/H09/" + id);
            return clip;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StopCombat();
            if (scene.name == "Combat") StartCombat();
            else music.clip = null;
        }
        public void StartCombat()
        {
            music.clip = Clip("battle_loop");
            if (music.clip != null) music.Play();
        }
        public void Ui(string id)
        {
            var clip = Clip(id);
            if (clip != null) ui.PlayOneShot(clip);
        }
        public void Combat(string id)
        {
            var clip = Clip(id);
            if (clip == null) return;
            AudioSource source = null;
            foreach (var candidate in combat) if (!candidate.isPlaying) { source = candidate; break; }
            if (source == null) source = combat[0];
            source.Stop(); source.clip = clip; source.Play();
            if (paused) source.Pause();
        }
        public void SetPaused(bool value)
        {
            if (paused == value) return;
            paused = value;
            if (value) { music.Pause(); foreach (var source in combat) source.Pause(); }
            else { music.UnPause(); foreach (var source in combat) source.UnPause(); }
        }
        public void StopCombat()
        {
            paused = false;
            foreach (var source in combat) { source.Stop(); source.clip = null; }
            music.Stop();
        }
        public void Started(PendingCombatAction action)
        {
            switch (action.Kind)
            {
                case CombatAction.Basic: Combat(action.Actor.Definition.Id == "warrior" ? "sword_swing" : action.Actor.Definition.Id == "ogre" ? "ogre_swing" : action.Actor.Definition.Id == "mage" ? "magic_cast" : "shaman_cast"); break;
                case CombatAction.HeavyStrike: case CombatAction.Cleave: Combat("heavy_swing"); break;
                case CombatAction.ArcaneStorm: Combat("storm_charge"); break;
                case CombatAction.Poison: Combat("shaman_cast"); break;
                case CombatAction.Heal: case CombatAction.DarkHeal: Combat("heal_cast"); break;
                default: Combat("item_use"); break;
            }
        }
        public void Impact(PendingCombatAction action)
        {
            string id;
            switch (action.Kind)
            {
                case CombatAction.Basic: id = action.Actor.Definition.Id == "warrior" ? "sword_hit" : action.Actor.Definition.Id == "ogre" ? "ogre_hit" : "magic_hit"; break;
                case CombatAction.HeavyStrike: case CombatAction.Cleave: id = "heavy_hit"; break;
                case CombatAction.ArcaneStorm: id = "storm_impact"; break;
                case CombatAction.Heal: case CombatAction.DarkHeal: case CombatAction.Potion: id = "heal_hit"; break;
                case CombatAction.Ether: id = "mana_hit"; break;
                case CombatAction.Poison: id = "poison_apply"; break;
                case CombatAction.HasteTonic: id = "haste_apply"; break;
                case CombatAction.CorrosiveFlask: id = "corrosion_apply"; break;
                case CombatAction.Antidote: id = "cleanse"; break;
                default: id = "magic_hit"; break;
            }
            Combat(id);
            foreach (var impact in action.Impacts)
                if (!impact.Target.IsAlive) Combat("death");
        }
        public void Result(CombatOutcome outcome)
        {
            music.Stop();
            foreach (var source in combat) source.Stop();
            Combat(outcome == CombatOutcome.Victory ? "victory" : "defeat");
        }
    }
}
