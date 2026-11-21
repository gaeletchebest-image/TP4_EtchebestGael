using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaGuardia
{
    public enum CombatTeam { Allies, Enemies }
    // Append only: existing ScriptableObjects serialize these numeric values.
    public enum CombatAction { Basic, HeavyStrike, Potion, Heal, ArcaneStorm, Ether, HasteTonic, CorrosiveFlask, Antidote, Cleave, Poison, DarkHeal }
    public enum TargetMode { Enemy, Ally, AllEnemies }
    public enum CombatOutcome { Ongoing, Victory, Defeat }

    // Immutable per-attempt snapshots keep authoring assets untouched.
    public sealed class ActionDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public CombatAction Command { get; }
        public int ManaCost { get; }
        public int Amount { get; }
        public double Potency { get; }
        public double Recovery { get; }
        public TargetMode TargetMode { get; }
        public ActionEffect Effect { get; }
        public EffectDefinition AppliedEffect { get; }
        public ActionDefinition(ActionData data)
        {
            Id = data.id; Name = data.displayName; Description = data.description;
            Command = data.command; ManaCost = data.manaCost; Amount = data.fixedAmount;
            Potency = data.potency; Recovery = data.recovery; TargetMode = data.targetMode; Effect = data.effect;
            AppliedEffect = data.appliedEffect == null ? null : new EffectDefinition(data.appliedEffect);
        }
    }
    public sealed class ItemDefinition
    {
        public CombatAction Command { get; }
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public int Amount { get; }
        public double Recovery { get; }
        public TargetMode TargetMode { get; }
        public ItemEffect Effect { get; }
        public EffectDefinition AppliedEffect { get; }
        public ItemDefinition(ItemData data)
        { Command = data.command; Id = data.id; Name = data.displayName; Description = data.description; Amount = data.amount; Recovery = data.recovery; TargetMode = data.targetMode; Effect = data.effect;
            AppliedEffect = data.appliedEffect == null ? null : new EffectDefinition(data.appliedEffect); }
    }
    public sealed class CombatantDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public CombatTeam Team { get; }
        public int MaxHealth { get; }
        public int MaxMana { get; }
        public int Attack { get; }
        public int Defense { get; }
        public double Speed { get; }
        public EnemyBrain Brain { get; }
        public IReadOnlyList<ActionDefinition> Actions { get; }
        public CombatantDefinition(CombatantData data)
        {
            Id = data.id; Name = data.displayName; Team = data.team; MaxHealth = data.maxHealth;
            MaxMana = data.maxMana; Attack = data.attack; Defense = data.defense; Speed = data.speed; Brain = data.brain;
            Actions = Array.AsReadOnly(data.actions.Select(a => new ActionDefinition(a)).ToArray());
        }
    }
    public sealed class CombatantState
    {
        public CombatantDefinition Definition { get; }
        public int Health { get; private set; }
        public int Mana { get; private set; }
        public double Atb { get; internal set; }
        public double RecoveryRemaining { get; internal set; }
        private readonly List<StatusState> effects = new List<StatusState>();
        private long applicationId;
        public IReadOnlyList<StatusState> Effects { get; }
        public bool HasStatus(StatusKind kind) => effects.Any(e => e.Definition.Kind == kind);
        private double Multiplier(StatusKind kind) => effects.FirstOrDefault(e => e.Definition.Kind == kind)?.Definition.Multiplier ?? 1;
        public double EffectiveSpeed => Definition.Speed * Multiplier(StatusKind.Haste);
        public double EffectiveDefense => Definition.Defense * Multiplier(StatusKind.Corrosion);
        public int AiActionsCompleted { get; internal set; }
        public bool IsAlive => Health > 0;
        public event Action Changed;
        public CombatantState(CombatantDefinition definition)
        { Definition = definition ?? throw new ArgumentNullException(nameof(definition)); Health = definition.MaxHealth; Mana = definition.MaxMana; Effects = effects.AsReadOnly(); }
        public bool ApplyStatus(EffectDefinition definition)
        {
            if (!IsAlive || definition == null) return false;
            var previous = effects.FirstOrDefault(e => e.Definition.Kind == definition.Kind);
            // A refresh keeps the original potency, gets a fresh identity and full duration.
            if (previous != null) effects.Remove(previous);
            effects.Add(new StatusState(previous?.Definition ?? definition, ++applicationId));
            Changed?.Invoke(); return true;
        }
        public bool RemoveStatus(StatusKind kind)
        {
            bool removed = effects.RemoveAll(e => e.Definition.Kind == kind) > 0;
            if (removed) Changed?.Invoke();
            return removed;
        }
        internal bool ConsumeCharge(StatusState effect)
        {
            if (!effects.Contains(effect)) return false;
            if (--effect.Remaining == 0) effects.Remove(effect);
            Changed?.Invoke(); return true;
        }
        public int Damage(int amount)
        {
            int actual = Math.Min(Health, Math.Max(0, amount)); Health -= actual;
            if (!IsAlive) { Atb = 0; RecoveryRemaining = 0; effects.Clear(); }
            if (actual > 0) Changed?.Invoke();
            return actual;
        }
        public int Heal(int amount)
        {
            if (!IsAlive) return 0;
            int actual = Math.Min(Definition.MaxHealth - Health, Math.Max(0, amount)); Health += actual;
            if (actual > 0) Changed?.Invoke();
            return actual;
        }
        public int RestoreMana(int amount)
        {
            if (!IsAlive) return 0;
            int actual = Math.Min(Definition.MaxMana - Mana, Math.Max(0, amount)); Mana += actual;
            if (actual > 0) Changed?.Invoke();
            return actual;
        }
        internal void SpendMana(int amount)
        {
            if (amount < 0 || amount > Mana) throw new InvalidOperationException("Maná inválido.");
            Mana -= amount;
            if (amount > 0) Changed?.Invoke();
        }
    }
    public sealed class PendingCombatAction
    {
        public CombatantState Actor { get; }
        public CombatantState Target { get; }
        public CombatAction Kind { get; }
        public double Recovery { get; }
        public ActionDefinition Action { get; }
        public ItemDefinition Item { get; }
        public bool ImpactApplied { get; internal set; }
        public int EffectiveAmount { get; internal set; }
        public IReadOnlyList<CombatantState> Targets { get; }
        private readonly List<CombatImpact> impacts = new List<CombatImpact>();
        public IReadOnlyList<CombatImpact> Impacts { get; }
        internal void Record(CombatantState target, int amount) { impacts.Add(new CombatImpact(target, amount)); EffectiveAmount += amount; }
        internal PendingCombatAction(CombatantState actor, CombatantState target, CombatAction kind, ActionDefinition action, ItemDefinition item, IEnumerable<CombatantState> targets)
        {
            Actor = actor; Target = target; Kind = kind; Action = action; Item = item;
            Recovery = item != null ? item.Recovery : action.Recovery;
            Targets = Array.AsReadOnly(targets.ToArray()); Impacts = impacts.AsReadOnly();
        }
    }

    public sealed class CombatImpact
    {
        public CombatantState Target { get; }
        public int Amount { get; }
        internal CombatImpact(CombatantState target, int amount) { Target = target; Amount = amount; }
    }

    // Owns the active clock, ready queue, validation, spending and impact; no Unity frame timing.
    public sealed class CombatSession
    {
        private sealed class ReadyEntry { public CombatantState Fighter; public double Crossing, Speed; }
        private readonly CombatantState[] fighters;
        private readonly Random random;
        private readonly List<ReadyEntry> ready = new List<ReadyEntry>();
        private readonly Dictionary<string, int> inventory;
        private readonly Dictionary<string, ItemDefinition> items;
        private StatusState[] turnEffects = Array.Empty<StatusState>();
        private bool startingTurn;
        public IReadOnlyList<CombatantState> Fighters { get; }
        public IReadOnlyDictionary<string, int> Inventory { get; }
        public CombatantState Actor { get; private set; }
        public PendingCombatAction Pending { get; private set; }
        public CombatOutcome Outcome { get; private set; }
        public bool IsPaused { get; set; }
        public double ActiveTime { get; private set; }
        public int ReadyCount => ready.Count;
        public bool ClockRunning => !IsPaused && Actor == null && Pending == null && ready.Count == 0 && Outcome == CombatOutcome.Ongoing;
        public int Potions => inventory.TryGetValue("potion", out int count) ? count : 0;
        public int CompletedActions { get; private set; }
        public string LastEvent { get; private set; } = "Defendé la entrada. Derrotá al ogro y al chamán.";

        public CombatSession(EncounterData encounter, int? seed = null)
        {
            if (encounter == null) throw new ArgumentNullException(nameof(encounter));
            var errors = encounter.ValidateData();
            if (errors.Count > 0) throw new ArgumentException(string.Join("\n", errors), nameof(encounter));
            random = seed.HasValue ? new Random(seed.Value) : new Random();
            fighters = encounter.participants.Select(s => new CombatantState(new CombatantDefinition(s.combatant))).ToArray();
            Fighters = Array.AsReadOnly(fighters);
            inventory = encounter.inventory.ToDictionary(e => e.item.id, e => e.quantity);
            Inventory = new System.Collections.ObjectModel.ReadOnlyDictionary<string, int>(inventory);
            items = encounter.inventory.ToDictionary(e => e.item.id, e => new ItemDefinition(e.item));
            foreach (var fighter in fighters) fighter.Changed += ReconcileDeaths;
        }
        public void AdvanceTime(double seconds)
        {
            if (double.IsNaN(seconds) || double.IsInfinity(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            ReconcileDeaths();
            if (!ClockRunning || seconds == 0) return;
            foreach (var f in fighters.Where(f => f.IsAlive))
            {
                double recovery = f.RecoveryRemaining;
                double crossing = recovery + (100 - f.Atb) / f.EffectiveSpeed;
                f.RecoveryRemaining = Math.Max(0, recovery - seconds);
                f.Atb = Math.Min(100, f.Atb + f.EffectiveSpeed * Math.Max(0, seconds - recovery));
                if (crossing <= seconds + 1e-10)
                { f.Atb = 100; ready.Add(new ReadyEntry { Fighter = f, Crossing = ActiveTime + crossing, Speed = f.EffectiveSpeed }); }
            }
            ActiveTime += seconds;
            ready.Sort((a, b) =>
            {
                if (Math.Abs(a.Crossing - b.Crossing) > 1e-9) return a.Crossing.CompareTo(b.Crossing);
                int speed = b.Speed.CompareTo(a.Speed);
                return speed != 0 ? speed : TieOrder(a.Fighter).CompareTo(TieOrder(b.Fighter));
            });
            BeginNextTurn();
        }
        private static int TieOrder(CombatantState f)
        {
            switch (f.Definition.Id) { case "mage": return 0; case "warrior": return 1; case "shaman": return 2; case "ogre": return 3; default: return 4; }
        }
        private void BeginNextTurn()
        {
            if (Outcome != CombatOutcome.Ongoing || Pending != null || Actor != null) return;
            while (ready.Count > 0)
            {
                var next = ready[0].Fighter; ready.RemoveAt(0);
                if (!next.IsAlive) continue;
                Actor = next;
                turnEffects = next.Effects.ToArray();
                // Damage raises Changed synchronously. Do not dispatch a second turn from that callback.
                startingTurn = true;
                try
                {
                    foreach (var effect in turnEffects.Where(e => e.Definition.Trigger == EffectTrigger.TurnStart))
                    {
                        if (!next.IsAlive) break;
                        int amount = next.Damage(effect.Definition.FixedDamage);
                        next.ConsumeCharge(effect);
                        LastEvent = next.Definition.Name + ": " + effect.Definition.Name + " −" + amount + " vida" +
                            (!next.IsAlive ? " (Derrotado)" : " · " + effect.Remaining + " cargas restantes");
                    }
                    ReconcileDeaths();
                }
                finally { startingTurn = false; }
                if (Outcome != CombatOutcome.Ongoing) { Actor = null; return; }
                if (next.IsAlive) return;
                Actor = null; turnEffects = Array.Empty<StatusState>();
            }
        }
        private void ReconcileDeaths()
        {
            ready.RemoveAll(e => !e.Fighter.IsAlive);
            if (!fighters.Any(f => f.Definition.Team == CombatTeam.Enemies && f.IsAlive)) Outcome = CombatOutcome.Victory;
            else if (!fighters.Any(f => f.Definition.Team == CombatTeam.Allies && f.IsAlive)) Outcome = CombatOutcome.Defeat;
            if (Outcome != CombatOutcome.Ongoing) { ready.Clear(); if (Pending == null) Actor = null; return; }
            if (!startingTurn && Actor != null && !Actor.IsAlive && Pending == null) { Actor = null; BeginNextTurn(); }
        }
        public static int CalculateDamage(int attack, double potency, double defense) =>
            Math.Max(1, (int)Math.Round(attack * potency - defense, MidpointRounding.AwayFromZero));
        public ActionDefinition ActionFor(CombatAction command) => Actor?.Definition.Actions.FirstOrDefault(a => a.Command == command);
        public ItemDefinition Potion => items["potion"];
        public ItemDefinition ItemFor(CombatAction command) => items.Values.FirstOrDefault(i => i.Command == command);
        public int QuantityFor(CombatAction command) => ItemFor(command) is ItemDefinition item ? inventory[item.Id] : 0;
        public static bool IsItem(CombatAction command) => command == CombatAction.Potion || command == CombatAction.Ether ||
            command == CombatAction.HasteTonic || command == CombatAction.CorrosiveFlask || command == CombatAction.Antidote;
        public CombatAction[] AvailableCommands() => Actor == null ? new CombatAction[0] : Actor.Definition.Actions.Select(a => a.Command)
            .Concat(Actor.Definition.Team == CombatTeam.Allies ? items.Values.Select(i => i.Command) : Enumerable.Empty<CombatAction>()).ToArray();
        public double RecoveryFor(CombatAction command) => IsItem(command) ? ItemFor(command)?.Recovery ?? 0 : ActionFor(command)?.Recovery ?? 0;
        public string NameFor(CombatAction command) => IsItem(command) ? ItemFor(command)?.Name ?? "Objeto no disponible" : ActionFor(command)?.Name ?? "Acción no disponible";
        public bool IsArea(CombatAction command) => !IsItem(command) && ActionFor(command)?.TargetMode == TargetMode.AllEnemies;

        public string Describe(CombatAction command)
        {
            var item = ItemFor(command);
            var action = ActionFor(command);
            if (IsItem(command))
                return item == null ? "Objeto no disponible." : item.Description + "\nUnidades: " + QuantityFor(command) + " · Maná: 0 · Recuperación: " + item.Recovery.ToString("0.0") + " s";
            if (action == null) return "Acción no disponible.";
            return action.Description + "\n" + (action.Effect == ActionEffect.Heal ? "Recupera " + action.Amount + " vida" :
                action.Effect == ActionEffect.Damage ? "Potencia " + action.Potency.ToString("0.##") : "Sin daño directo") +
                " · Maná: " + action.ManaCost + " · Recuperación: " + action.Recovery.ToString("0.0") + " s";
        }
        public List<CombatantState> ResolveTargets(CombatantState actor, TargetMode mode)
        {
            if (actor == null || !actor.IsAlive || !fighters.Contains(actor)) return new List<CombatantState>();
            return fighters.Where(f => f.IsAlive && (mode == TargetMode.Ally ? f.Definition.Team == actor.Definition.Team : f.Definition.Team != actor.Definition.Team)).ToList();
        }
        public List<CombatantState> TargetsFor(CombatAction command)
        {
            var action = ActionFor(command);
            var item = ItemFor(command);
            if (Actor == null || (IsItem(command) ? item == null || Actor.Definition.Team != CombatTeam.Allies : action == null)) return new List<CombatantState>();
            var targets = ResolveTargets(Actor, IsItem(command) ? item.TargetMode : action.TargetMode);
            bool healing = IsItem(command) ? item.Effect == ItemEffect.Heal : action.Effect == ActionEffect.Heal;
            bool mana = IsItem(command) ? item.Effect == ItemEffect.RestoreMana : action.Effect == ActionEffect.RestoreMana;
            if (healing) targets.RemoveAll(f => f.Health == f.Definition.MaxHealth);
            if (mana) targets.RemoveAll(f => f.Mana == f.Definition.MaxMana);
            if (command == CombatAction.DarkHeal) targets.RemoveAll(f => f.Definition.Id != "ogre");
            if (command == CombatAction.Antidote) targets.RemoveAll(f => !f.HasStatus(StatusKind.Poison));
            if (command == CombatAction.Poison) targets.RemoveAll(f => f.HasStatus(StatusKind.Poison));
            return targets;
        }
        public string UnavailableReason(CombatAction command)
        {
            if (Outcome != CombatOutcome.Ongoing) return "El combate terminó.";
            if (IsPaused) return "Combate pausado.";
            if (Pending != null) return "Acción en ejecución.";
            if (Actor == null) return "Esperando ATB.";
            if (!Actor.IsAlive) return "El participante está derrotado.";
            if (!Enum.IsDefined(typeof(CombatAction), command)) return "Acción desconocida.";
            if (IsItem(command))
            {
                var item = ItemFor(command);
                if (Actor.Definition.Team != CombatTeam.Allies || item == null) return "Objeto no disponible.";
                if (QuantityFor(command) == 0) return "Sin unidades.";
            }
            else
            {
                var action = ActionFor(command);
                if (action == null) return "Habilidad no disponible.";
                if (Actor.Mana < action.ManaCost) return "Maná insuficiente.";
            }
            return TargetsFor(command).Count == 0 ? "No hay objetivos válidos." : null;
        }
        public bool TryCommit(CombatAction command, CombatantState target, out string reason)
        {
            ReconcileDeaths(); reason = UnavailableReason(command);
            if (reason != null) return false;
            if (!TargetsFor(command).Contains(target)) { reason = "Objetivo inválido."; return false; }
            var targets = IsArea(command) ? TargetsFor(command).ToArray() : new[] { target };
            Pending = new PendingCombatAction(Actor, target, command, ActionFor(command), IsItem(command) ? ItemFor(command) : null, targets);
            if (IsItem(command)) inventory[Pending.Item.Id]--;
            else Actor.SpendMana(Pending.Action.ManaCost);
            if (Pending.Item?.Effect == ItemEffect.RemovePoison) target.RemoveStatus(StatusKind.Poison);
            LastEvent = Actor.Definition.Name + " prepara " + NameFor(command) + " → " + string.Join(", ", targets.Select(t => t.Definition.Name));
            return true;
        }
        public bool CommitEnemyAction()
        {
            if (Actor == null || Actor.Definition.Team != CombatTeam.Enemies || Pending != null || Outcome != CombatOutcome.Ongoing) return false;
            CombatAction choice = CombatAction.Basic;
            if (Actor.Definition.Brain == EnemyBrain.Ogre && Actor.AiActionsCompleted % 2 == 1 && UnavailableReason(CombatAction.Cleave) == null)
                choice = CombatAction.Cleave;
            else if (Actor.Definition.Brain == EnemyBrain.Shaman)
            {
                var ogre = fighters.FirstOrDefault(f => f.Definition.Id == "ogre" && f.IsAlive);
                if (ogre != null && ogre.Health < ogre.Definition.MaxHealth * 0.5 && UnavailableReason(CombatAction.DarkHeal) == null)
                    choice = CombatAction.DarkHeal;
                else if (UnavailableReason(CombatAction.Poison) == null) choice = CombatAction.Poison;
            }
            var targets = TargetsFor(choice);
            return targets.Count > 0 && TryCommit(choice, targets[random.Next(targets.Count)], out _);
        }
        public bool ApplyImpact()
        {
            if (IsPaused || Pending == null || Pending.ImpactApplied || Outcome != CombatOutcome.Ongoing) return false;
            Pending.ImpactApplied = true;
            bool healing = Pending.Item != null ? Pending.Item.Effect == ItemEffect.Heal : Pending.Action.Effect == ActionEffect.Heal;
            bool mana = Pending.Item != null ? Pending.Item.Effect == ItemEffect.RestoreMana : Pending.Action.Effect == ActionEffect.RestoreMana;
            bool status = Pending.Item != null ? Pending.Item.Effect == ItemEffect.ApplyStatus : Pending.Action.Effect == ActionEffect.ApplyStatus;
            bool cleanse = Pending.Item?.Effect == ItemEffect.RemovePoison;
            var effect = Pending.Item != null ? Pending.Item.AppliedEffect : Pending.Action.AppliedEffect;
            int fixedAmount = Pending.Item?.Amount ?? Pending.Action.Amount;
            foreach (var target in Pending.Targets.Where(t => t.IsAlive))
            {
                if (status) target.ApplyStatus(effect);
                int amount = status || cleanse ? 0 : healing ? target.Heal(fixedAmount) : mana ? target.RestoreMana(fixedAmount) :
                    target.Damage(CalculateDamage(Pending.Actor.Definition.Attack, Pending.Action.Potency, target.EffectiveDefense));
                Pending.Record(target, amount);
            }
            LastEvent = Pending.Actor.Definition.Name + ": " + NameFor(Pending.Kind) + " → " + string.Join(" · ", Pending.Impacts.Select(i =>
                i.Target.Definition.Name + (status ? " · " + effect.Name + " (" + effect.Duration + ")" : cleanse ? " · Veneno eliminado" :
                    (healing || mana ? " +" : " −") + i.Amount + (mana ? " maná" : " vida")) + (i.Target.IsAlive ? "" : " (Derrotado)")));
            ReconcileDeaths(); return true;
        }
        public bool FinishAction()
        {
            if (IsPaused || Pending == null || !Pending.ImpactApplied) return false;
            var actor = Pending.Actor;
            actor.Atb = 0; actor.RecoveryRemaining = actor.IsAlive ? Pending.Recovery : 0;
            if (actor.Definition.Team == CombatTeam.Enemies) actor.AiActionsCompleted++;
            foreach (var effect in turnEffects.Where(e => e.Definition.Trigger == EffectTrigger.ActionEnd))
                if (actor.ConsumeCharge(effect) && effect.Remaining == 0) LastEvent += " · Fin de " + effect.Definition.Name + " en " + actor.Definition.Name;
            turnEffects = Array.Empty<StatusState>();
            Pending = null; Actor = null; CompletedActions++;
            ReconcileDeaths(); BeginNextTurn(); return true;
        }
        public static string ActionName(CombatAction command, CombatantState actor) => command == CombatAction.Potion ? "Poción" :
            actor?.Definition.Actions.FirstOrDefault(a => a.Command == command)?.Name ?? "Acción no disponible";
    }
}
