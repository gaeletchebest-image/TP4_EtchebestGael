using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UltimaGuardia
{
    public sealed class FinalCombatHud : MonoBehaviour
    {
        [Serializable] public sealed class FighterPanel
        {
            public string id;
            public Text name, life, mana, clock;
            public Image lifeBar, manaBar, atbBar, edge, clockIcon;
            public Text[] states;
            public Image[] stateIcons;
        }
        [SerializeField] private FighterPanel[] fighters;
        [SerializeField] private Text[] actionLabels;
        [SerializeField] private Image[] actionIcons, actionBackgrounds;
        [SerializeField] private Text logLabel, contextLabel, overlayHint, commandSummary;
        [SerializeField] private RectTransform floatRoot;
        [SerializeField] private Image commandIcon, overlaySelection, deviceIndicator, overlayDeviceIndicator;
        private sealed class Floating { public Text text; public float age; public Vector2 origin; public CombatantState fighter; public int offsetY; }
        private readonly List<Floating> numbers = new List<Floating>();
        private CombatSession bound;
        public int VisibleNumbers => numbers.Count;
        public void Bind(CombatSession session)
        {
            if (bound != null) bound.ResourceFeedback -= OnFeedback;
            foreach (var n in numbers) if (n.text != null) Destroy(n.text.gameObject);
            numbers.Clear(); bound = session; bound.ResourceFeedback += OnFeedback;
        }
        private void OnDestroy() { if (bound != null) bound.ResourceFeedback -= OnFeedback; }
        private void OnFeedback(CombatantState fighter, int amount, bool mana)
        {
            var view = FindObjectsByType<CombatantView>(FindObjectsSortMode.None).First(v => v.combatantId == fighter.Definition.Id);
            var viewport = Camera.main.WorldToViewportPoint(view.effectAnchor.position);
            var rect = floatRoot.rect;
            int overlapping = numbers.Count(n => Mathf.Abs(n.origin.x - (viewport.x - .5f) * rect.width) < 190);
            Vector2 point = new Vector2((viewport.x - .5f) * rect.width, (viewport.y - .5f) * rect.height + 32 + overlapping * 44);
            point.x = Mathf.Clamp(point.x, -rect.width / 2 + 100, rect.width / 2 - 100);
            point.y = Mathf.Clamp(point.y, -95, 110);
            var text = InterfaceTheme.Text(floatRoot, "ResourceFeedback", (amount > 0 ? "+" : "−") + Math.Abs(amount) + (mana ? " MANÁ" : " VIDA"), 23, point.x, point.y, 200, 34, TextAnchor.MiddleCenter);
            text.fontStyle = FontStyle.Bold; text.color = mana ? InterfaceTheme.Mana : amount > 0 ? InterfaceTheme.Life : Color.white;
            var shadow = text.gameObject.AddComponent<Outline>(); shadow.effectColor = Color.black; shadow.effectDistance = new Vector2(2, -2);
            numbers.Add(new Floating { text = text, origin = point, fighter = fighter, offsetY = overlapping * 44 });
        }
        public void Present(CombatPrototype battle)
        {
            for (int i = numbers.Count - 1; i >= 0; i--)
            {
                var n = numbers[i];
                if (n.age == 0 && !battle.IsCinematicHidden)
                {
                    var view = FindObjectsByType<CombatantView>(FindObjectsSortMode.None).First(v => v.combatantId == n.fighter.Definition.Id);
                    var viewport = Camera.main.WorldToViewportPoint(view.effectAnchor.position);
                    var rect = floatRoot.rect;
                    n.origin = new Vector2(Mathf.Clamp((viewport.x - .5f) * rect.width, -rect.width / 2 + 100, rect.width / 2 - 100),
                        Mathf.Clamp((viewport.y - .5f) * rect.height + 32 + n.offsetY, -95, 110));
                }
                if (!battle.IsPaused && !battle.IsCinematicHidden) n.age += Time.deltaTime;
                if (n.age > 1.5f) { Destroy(n.text.gameObject); numbers.RemoveAt(i); continue; }
                n.text.rectTransform.anchoredPosition = n.origin + Vector2.up * n.age * 20;
            }
            foreach (var p in fighters)
            {
                var f = battle.Session.Fighters.First(s => s.Definition.Id == p.id);
                bool active = f == battle.Session.Actor && battle.Session.Outcome == CombatOutcome.Ongoing;
                p.name.text = f.Definition.Name + "  /  " + (f.Definition.Team == CombatTeam.Allies ? "ALIADO" : "ENEMIGO");
                p.name.color = f.IsAlive ? Color.white : InterfaceTheme.Muted;
                p.life.text = "Vida " + f.Health + " / " + f.Definition.MaxHealth;
                p.mana.text = "Maná " + f.Mana + " / " + f.Definition.MaxMana;
                p.lifeBar.fillAmount = (float)f.Health / f.Definition.MaxHealth;
                p.manaBar.fillAmount = f.Definition.MaxMana == 0 ? 0 : (float)f.Mana / f.Definition.MaxMana;
                p.atbBar.fillAmount = (float)(f.Atb / 100);
                p.atbBar.color = f.Atb >= 100 ? InterfaceTheme.Gold : InterfaceTheme.Mana;
                p.clockIcon.sprite = Resources.Load<Sprite>("Interface/" + (f.RecoveryRemaining > 0 ? "recovery" : "atb"));
                p.edge.color = active ? InterfaceTheme.Gold : new Color32(65, 85, 101, 255);
                p.clock.text = !f.IsAlive ? "DERROTADO" : f.Atb >= 100 ? active ? "LISTO · ACTÚA" : "LISTO · EN COLA" :
                    f.RecoveryRemaining > 0 ? "RECUPERANDO  " + f.RecoveryRemaining.ToString("0.0") + " s" : "ATB  " + f.Atb.ToString("0") + " % · LLENANDO";
                for (int i = 0; i < 3; i++)
                {
                    var effect = f.Effects.FirstOrDefault(e => (int)e.Definition.Kind == i);
                    p.states[i].gameObject.SetActive(effect != null); p.stateIcons[i].gameObject.SetActive(effect != null);
                    if (effect != null) p.states[i].text = effect.Definition.Name + " " + effect.Remaining;
                }
            }
            var actions = battle.DisplayedCommands;
            bool listing = battle.Phase == PrototypePhase.Commands && battle.Session.Actor?.Definition.Team == CombatTeam.Allies;
            bool root = battle.Menu == CombatMenu.Root;
            commandSummary.enabled = !listing;
            for (int i = 0; i < actionLabels.Length; i++)
            {
                bool visible = listing && i < (root ? 3 : actions.Length);
                actionLabels[i].gameObject.SetActive(visible); actionIcons[i].gameObject.SetActive(visible); actionBackgrounds[i].gameObject.SetActive(visible);
                if (!visible) continue;
                bool selected = i == battle.SelectedCommand;
                var a = root ? CombatAction.Basic : actions[i];
                string name = root ? new[] { "Atacar", "Habilidades", "Objetos" }[i] : battle.Session.NameFor(a);
                string cost = root ? "" : CombatSession.IsItem(a) ? " ×" + battle.Session.QuantityFor(a) : " · " + battle.Session.ActionFor(a).ManaCost + " PM";
                bool available = root || battle.Session.UnavailableReason(a) == null;
                actionLabels[i].text = (selected ? "› " : "  ") + name + cost;
                actionLabels[i].color = selected ? InterfaceTheme.Gold : available ? Color.white : InterfaceTheme.Muted;
                actionBackgrounds[i].color = selected ? new Color32(54, 66, 72, 255) : Color.clear;
                actionIcons[i].sprite = Resources.Load<Sprite>("Interface/" + (root ? new[] { battle.Session.Actor.Definition.Id == "mage" ? "magic" : "sword", "magic", "potion" }[i] : ActionIcon(a)));
                actionIcons[i].color = available ? InterfaceTheme.Gold : InterfaceTheme.Muted;
            }
            contextLabel.text = listing ? "ELEGÍ UNA ACCIÓN · RELOJ DETENIDO" : battle.Phase == PrototypePhase.Targets ? "ELEGÍ OBJETIVO · RELOJ DETENIDO" : battle.Phase == PrototypePhase.Executing ? "ACCIÓN EN CURSO · RELOJ DETENIDO" : battle.Phase == PrototypePhase.Result ? "LA DEFENSA TERMINÓ" : "AVANZAN ATB Y RECUPERACIÓN";
            // The four HUD identities stay visible; world labels yield briefly to numbers.
            foreach (var view in FindObjectsByType<CombatantView>(FindObjectsSortMode.None))
                view.nameLabel.gameObject.SetActive(numbers.Count == 0);
            commandIcon.sprite = Resources.Load<Sprite>("Interface/" + (battle.Phase == PrototypePhase.Targets ? "target" : "sword"));
            logLabel.text = string.Join("\n", battle.Session.CombatLog.Select(s => "· " + s));
            overlayHint.text = SessionOptions.Help;
            deviceIndicator.sprite = overlayDeviceIndicator.sprite = Resources.Load<Sprite>("Interface/" + (SessionOptions.UsingGamepad ? "gamepad" : "keyboard"));
            bool overlay = battle.IsPaused || battle.Phase == PrototypePhase.Result;
            bool menu = !battle.IsPaused || battle.PausePage == 0;
            overlaySelection.gameObject.SetActive(overlay && menu);
            if (overlay && menu) overlaySelection.rectTransform.anchoredPosition = new Vector2(-405, 172 - battle.OverlaySelection * 50);
        }
        public static string ActionIcon(CombatAction action) => action == CombatAction.Potion ? "potion" : action == CombatAction.Ether ? "ether" :
            action == CombatAction.HasteTonic ? "tonic" : action == CombatAction.CorrosiveFlask ? "flask" : action == CombatAction.Antidote ? "antidote" :
            action == CombatAction.Heal || action == CombatAction.DarkHeal ? "heal" : action == CombatAction.ArcaneStorm ? "storm" : action == CombatAction.Poison ? "poison" :
            action == CombatAction.HeavyStrike || action == CombatAction.Cleave ? "heavy" : "magic";

        // Called only by the explicit H08 authoring command, serialized into Combat.
        public void Build(Transform canvas, out Text[] resources, out Image[] bars, out Text turn, out Text events,
            out Text commands, out Text description, out Text help, out GameObject decision, out GameObject overlay,
            out Text title, out Text options)
        {
            InterfaceTheme.Box(canvas, "Header", 0, 333, 1280, 54, InterfaceTheme.Ink);
            turn = InterfaceTheme.Text(canvas, "Turn", "ÚLTIMA GUARDIA", 22, -280, 332, 670, 36); turn.color = InterfaceTheme.Gold;
            contextLabel = InterfaceTheme.Text(canvas, "ClockContext", "", 15, 375, 332, 480, 36, TextAnchor.MiddleRight); contextLabel.color = InterfaceTheme.Muted;
            fighters = new FighterPanel[4]; resources = new Text[4]; bars = new Image[4];
            string[] ids = { "warrior", "mage", "ogre", "shaman" };
            for (int i = 0; i < 4; i++)
            {
                var card = InterfaceTheme.Box(canvas, "Card_" + ids[i], -465 + i * 310, 240, 300, 140, InterfaceTheme.Ink).transform;
                if (i >= 2) card.localScale = Vector3.one * .94f;
                var p = fighters[i] = new FighterPanel { id = ids[i], states = new Text[3], stateIcons = new Image[3] };
                p.edge = InterfaceTheme.Box(card, "ActiveEdge", -149, 0, 2, 140, InterfaceTheme.Gold);
                p.name = InterfaceTheme.Text(card, "Name", "", 17, 6, 50, 276, 26);
                InterfaceTheme.Icon(card, ids[i], -110, 8, 54, InterfaceTheme.Gold);
                InterfaceTheme.Icon(card, "life", -68, 24, 16, InterfaceTheme.Life);
                InterfaceTheme.Icon(card, "mana", -68, -6, 16, InterfaceTheme.Mana);
                p.life = InterfaceTheme.Text(card, "Life", "", 16, 37, 24, 190, 24);
                p.mana = InterfaceTheme.Text(card, "Mana", "", 16, 37, -6, 190, 24);
                p.lifeBar = InterfaceTheme.Bar(card, "LifeBar", 38, 10, 188, InterfaceTheme.Life);
                p.manaBar = InterfaceTheme.Bar(card, "ManaBar", 38, -20, 188, InterfaceTheme.Mana);
                p.clockIcon = InterfaceTheme.Icon(card, "atb", -129, -37, 16, InterfaceTheme.Gold);
                p.clock = InterfaceTheme.Text(card, "Clock", "", 13, 14, -36, 254, 22); p.clock.color = InterfaceTheme.Muted;
                p.atbBar = InterfaceTheme.Bar(card, "AtbBar", 0, -49, 274, InterfaceTheme.Mana);
                for (int j = 0; j < 3; j++)
                {
                    p.stateIcons[j] = InterfaceTheme.Icon(card, new[] { "poison", "haste", "corrosion" }[j], -128 + j * 93, -61, 14, j == 0 ? InterfaceTheme.Life : j == 1 ? InterfaceTheme.Mana : InterfaceTheme.Danger);
                    p.states[j] = InterfaceTheme.Text(card, "State" + j, "", 13, -82 + j * 93, -61, 75, 20);
                }
                resources[i] = InterfaceTheme.Text(card, "CompatibilityResource", "", 1, 0, 0, 1, 1); resources[i].enabled = false;
                bars[i] = p.atbBar;
            }
            floatRoot = InterfaceTheme.Rect(canvas, "FloatingNumbers", 0, 0, 1280, 720);
            decision = InterfaceTheme.Box(canvas, "Decision", -207, -222, 826, 212, InterfaceTheme.Ink).gameObject;
            InterfaceTheme.Box(decision.transform, "Rule", -81, 0, 1, 176, new Color32(65, 85, 101, 255));
            commandIcon = InterfaceTheme.Icon(decision.transform, "sword", -386, 84, 20, InterfaceTheme.Gold);
            InterfaceTheme.Text(decision.transform, "ActionsTitle", "COMANDOS", 14, -252, 84, 220, 24).color = InterfaceTheme.Muted;
            actionLabels = new Text[5]; actionIcons = new Image[5]; actionBackgrounds = new Image[5];
            for (int i = 0; i < 5; i++)
            {
                actionBackgrounds[i] = InterfaceTheme.Box(decision.transform, "ActionRow" + i, -249, 50 - i * 30, 304, 29, Color.clear);
                actionIcons[i] = InterfaceTheme.Icon(decision.transform, "sword", -384, 50 - i * 30, 20, InterfaceTheme.Gold);
                actionLabels[i] = InterfaceTheme.Text(decision.transform, "Action" + i, "", 17, -224, 50 - i * 30, 282, 28);
            }
            commands = InterfaceTheme.Text(decision.transform, "Commands", "", 20, -247, 0, 300, 140);
            commandSummary = commands;
            description = InterfaceTheme.Text(decision.transform, "Description", "", 18, 169, 0, 455, 184);
            var log = InterfaceTheme.Box(canvas, "CombatLog", 421, -222, 398, 212, InterfaceTheme.Ink).transform;
            InterfaceTheme.Text(log, "LogTitle", "REGISTRO · ÚLTIMOS 4 EVENTOS", 14, 0, 84, 366, 26).color = InterfaceTheme.Gold;
            logLabel = InterfaceTheme.Text(log, "LogEvents", "", 15, 0, -14, 366, 166, TextAnchor.UpperLeft);
            events = InterfaceTheme.Text(canvas, "LastEvent", "", 1, 0, 0, 1, 1); events.enabled = false;
            help = InterfaceTheme.Text(canvas, "Help", "", 16, 0, -345, 1236, 30, TextAnchor.MiddleCenter); help.color = InterfaceTheme.Muted;
            deviceIndicator = InterfaceTheme.Icon(canvas, "keyboard", -600, -345, 24, InterfaceTheme.Gold);
            overlay = InterfaceTheme.Box(canvas, "Overlay", 0, 0, 1280, 720, new Color32(8, 15, 24, 242)).gameObject;
            var frame = InterfaceTheme.Box(overlay.transform, "Modal", 0, 20, 880, 584, InterfaceTheme.Panel).transform;
            InterfaceTheme.Box(frame, "GoldRule", 0, 291, 880, 2, InterfaceTheme.Gold);
            title = InterfaceTheme.MenuText(frame, "OverlayTitle", "", 36, 0, 222, 800, 72, TextAnchor.MiddleCenter); title.color = InterfaceTheme.Gold;
            options = InterfaceTheme.MenuText(frame, "OverlayOptions", "", 22, 0, -9, 780, 388, TextAnchor.UpperLeft);
            options.lineSpacing = 1;
            overlaySelection = InterfaceTheme.Box(frame, "Selection", -296, 107, 4, 32, InterfaceTheme.Gold);
            overlayHint = InterfaceTheme.MenuText(overlay.transform, "OverlayHelp", "", 16, 0, -320, 1240, 55, TextAnchor.MiddleCenter);
            overlayDeviceIndicator = InterfaceTheme.Icon(overlay.transform, "keyboard", -600, -320, 24, InterfaceTheme.Gold);
            overlay.SetActive(false);
        }
    }
}
