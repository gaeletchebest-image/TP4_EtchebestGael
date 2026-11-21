using UnityEngine;

namespace UltimaGuardia
{
    public enum EnemyBrain { None, BasicRandom, Ogre, Shaman }

    [CreateAssetMenu(menuName = "Ultima Guardia/Combatiente")]
    public sealed class CombatantData : ScriptableObject
    {
        public string id, displayName;
        public CombatTeam team;
        public int maxHealth = 1, maxMana, attack, defense;
        public float speed = 20;
        public GameObject prefab;
        public ActionData[] actions = new ActionData[0];
        public EnemyBrain brain;
    }
}
