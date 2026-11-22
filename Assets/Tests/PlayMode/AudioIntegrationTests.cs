using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UltimaGuardia.Tests
{
    public sealed class AudioIntegrationTests
    {
        [UnityTest]
        public IEnumerator AllCuesLoadAndCombatMusicSurvivesPauseThenClears()
        {
            string[] cues = { "battle_loop", "ui_move", "ui_confirm", "ui_cancel", "ui_invalid",
                "sword_swing", "ogre_swing", "heavy_swing", "sword_hit", "ogre_hit", "heavy_hit",
                "magic_cast", "shaman_cast", "magic_hit", "heal_cast", "heal_hit", "mana_hit",
                "item_use", "poison_apply", "poison_tick", "haste_apply", "corrosion_apply",
                "cleanse", "death", "storm_charge", "storm_impact", "victory", "defeat" };
            foreach (var cue in cues) Assert.IsNotNull(Resources.Load<AudioClip>("Audio/H09/" + cue), cue);
            var audio = GameAudio.Current;
            audio.StopCombat(); audio.StartCombat();
            yield return null;
            var sources = audio.GetComponents<AudioSource>();
            Assert.AreEqual("battle_loop", sources[0].clip.name);
            Assert.IsTrue(sources[0].loop);
            audio.Combat("storm_charge");
            Assert.AreEqual("storm_charge", sources[2].clip.name);
            int previousVolume = SessionOptions.Volume;
            SessionOptions.SetVolume(0);
            Assert.AreEqual(0f, AudioListener.volume);
            SessionOptions.SetVolume(100);
            Assert.AreEqual(1f, AudioListener.volume);
            SessionOptions.SetVolume(previousVolume);
            audio.SetPaused(true);
            audio.Ui("ui_confirm");
            audio.SetPaused(false);
            audio.StopCombat();
            Assert.IsNull(sources[2].clip);
            Assert.IsFalse(sources[0].isPlaying);
        }
    }
}
