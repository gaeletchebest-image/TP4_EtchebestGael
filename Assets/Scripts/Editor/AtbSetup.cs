using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UltimaGuardia.Editor
{
    public static class AtbSetup
    {
        public const string Root = "Assets/Data/H02/Resources/Encounters/";
        [MenuItem("Ultima Guardia/H02/Integrar datos y ATB en Combat")]
        public static void Install()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Detener Play y guardar la escena antes de integrar H02.");
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Combat.unity");
            Directory.CreateDirectory(Root);
            AssetDatabase.Refresh();
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>(Root + "MainEncounter.asset");
            if (encounter == null) encounter = CreateData();
            var errors = encounter.ValidateData();
            if (errors.Count != 0) throw new InvalidOperationException(string.Join("\n", errors));
            var controller = UnityEngine.Object.FindFirstObjectByType<CombatPrototype>();
            var canvas = GameObject.Find("CombatCanvas").transform;
            var bars = new Image[4];
            for (int i = 0; i < 4; i++)
            {
                string id = encounter.participants[i].combatant.id;
                var label = canvas.Find("Resources_" + id).GetComponent<Text>();
                label.rectTransform.anchoredPosition = new Vector2(-465 + i * 310, 260);
                label.rectTransform.sizeDelta = new Vector2(300, 102);
                label.fontSize = 18;
                string barName = "ATB_" + id;
                var existing = canvas.Find(barName);
                var background = existing != null ? existing.gameObject : new GameObject(barName, typeof(RectTransform), typeof(Image));
                background.transform.SetParent(canvas, false);
                background.transform.SetSiblingIndex(label.transform.GetSiblingIndex() + 1);
                var rect = background.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-465 + i * 310, 202);
                rect.sizeDelta = new Vector2(290, 10);
                background.GetComponent<Image>().color = new Color(0.12f, 0.20f, 0.27f);
                var fillTransform = background.transform.Find("Fill");
                var fill = fillTransform != null ? fillTransform.gameObject : new GameObject("Fill", typeof(RectTransform), typeof(Image));
                fill.transform.SetParent(background.transform, false);
                var fillRect = fill.GetComponent<RectTransform>();
                fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = Vector2.one;
                fillRect.offsetMin = Vector2.zero; fillRect.offsetMax = Vector2.zero;
                bars[i] = fill.GetComponent<Image>();
                bars[i].sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                bars[i].type = Image.Type.Filled; bars[i].fillMethod = Image.FillMethod.Horizontal; bars[i].fillOrigin = 0; bars[i].fillAmount = 0;
                bars[i].raycastTarget = false;
            }
            var header = canvas.Find("Header").GetComponent<RectTransform>();
            header.anchoredPosition = new Vector2(0, 275); header.sizeDelta = new Vector2(1280, 170);
            controller.ConfigureEncounter(encounter, bars);
            EditorUtility.SetDirty(controller);
            PlayerSettings.bundleVersion = "0.0.3";
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("H02: datos editables y barras integrados sin regenerar arena, prefabs ni MainMenu.");
        }

        private static T Asset<T>(string file, Action<T> initialize) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            asset.name = file;
            initialize(asset);
            AssetDatabase.CreateAsset(asset, Root + file + ".asset");
            return asset;
        }
        private static ActionData Attack(string id, string label, CombatAction command = CombatAction.Basic, int cost = 0, float potency = 1, float recovery = 0.6f) =>
            Asset<ActionData>(id, a => { a.id = id; a.displayName = label; a.description = "Un enemigo vivo"; a.command = command; a.manaCost = cost; a.potency = potency; a.recovery = recovery; a.targetMode = TargetMode.Enemy; });
        private static CombatantData Fighter(string id, string label, CombatTeam team, int hp, int mp, int attack, int defense, int speed, params ActionData[] actions) =>
            Asset<CombatantData>(id, c => { c.id = id; c.displayName = label; c.team = team; c.maxHealth = hp; c.maxMana = mp; c.attack = attack; c.defense = defense; c.speed = speed; c.actions = actions;
                c.prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/H01/" + id + ".prefab"); c.brain = team == CombatTeam.Enemies ? EnemyBrain.BasicRandom : EnemyBrain.None; });
        private static EffectData Effect(string id, string label, StatusKind kind, int duration, int damage, float multiplier, EffectTrigger trigger) =>
            Asset<EffectData>(id, e => { e.id = id; e.displayName = label; e.kind = kind; e.duration = duration; e.fixedDamage = damage; e.multiplier = multiplier; e.trigger = trigger; });
        private static ItemData Item(string id, string label, ItemEffect kind, int amount, TargetMode targets, EffectData status = null) =>
            Asset<ItemData>(id, item => { item.id = id; item.displayName = label; item.description = kind == ItemEffect.Heal ? "Un aliado vivo herido" : "Configuración preparada; ejecución en H03/H04";
                item.command = id == "potion" ? CombatAction.Potion : id == "ether" ? CombatAction.Ether : id == "haste_tonic" ? CombatAction.HasteTonic : id == "corrosive_flask" ? CombatAction.CorrosiveFlask : CombatAction.Antidote;
                item.effect = kind; item.amount = amount; item.targetMode = targets; item.appliedEffect = status; });
        private static EncounterData CreateData()
        {
            var basic = Attack("warrior_basic", "Ataque básico");
            var heavy = Attack("heavy_strike", "Golpe pesado", CombatAction.HeavyStrike, 8, 1.8f, 2.2f);
            var magic = Attack("mage_basic", "Proyectil mágico");
            var hit = Attack("ogre_basic", "Golpe");
            var discharge = Attack("shaman_basic", "Descarga");
            var fighters = new[] {
                Fighter("warrior", "Guerrero", CombatTeam.Allies, 160, 30, 28, 8, 22, basic, heavy),
                Fighter("mage", "Maga", CombatTeam.Allies, 110, 45, 24, 4, 28, magic),
                Fighter("ogre", "Ogro", CombatTeam.Enemies, 240, 24, 28, 6, 16, hit),
                Fighter("shaman", "Chamán", CombatTeam.Enemies, 130, 36, 18, 4, 20, discharge) };
            var poison = Effect("poison", "Veneno", StatusKind.Poison, 3, 8, 1, EffectTrigger.TurnStart);
            var haste = Effect("haste", "Rapidez", StatusKind.Haste, 2, 0, 1.5f, EffectTrigger.ActionEnd);
            var corrosion = Effect("corrosion", "Corrosión", StatusKind.Corrosion, 2, 0, 0.5f, EffectTrigger.ActionEnd);
            var items = new[] { Item("potion", "Poción", ItemEffect.Heal, 50, TargetMode.Ally), Item("ether", "Éter", ItemEffect.RestoreMana, 20, TargetMode.Ally),
                Item("haste_tonic", "Tónico de rapidez", ItemEffect.ApplyStatus, 0, TargetMode.Ally, haste), Item("corrosive_flask", "Frasco corrosivo", ItemEffect.ApplyStatus, 0, TargetMode.Enemy, corrosion),
                Item("antidote", "Antídoto", ItemEffect.RemovePoison, 0, TargetMode.Ally, poison) };
            int[] quantities = { 3, 2, 1, 1, 2 };
            return Asset<EncounterData>("MainEncounter", e => {
                e.participants = fighters.Select(c => { var anchor = GameObject.Find("Anchor_" + c.id).transform;
                    return new EncounterSlot { combatant = c, position = anchor.position, rotation = anchor.eulerAngles }; }).ToArray();
                e.inventory = items.Select((item, i) => new InitialItem { item = item, quantity = quantities[i] }).ToArray();
            });
        }
    }
}
