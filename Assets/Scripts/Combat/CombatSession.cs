using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaGuardia
{
    public enum CombatTeam { Allies, Enemies }
    public enum CombatAction { Basic, HeavyStrike, Potion }
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
        public ActionDefinition(ActionData data)
        {
            Id = data.id; Name = data.displayName; Description = data.description;
            Command = data.command; ManaCost = data.manaCost; Amount = data.fixedAmount;
            Potency = data.potency; Recovery = data.recovery; TargetMode = data.targetMode; Effect = data.effect;
        }
    }
    public sealed class ItemDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public int Amount { get; }
        public double Recovery { get; }
        public TargetMode TargetMode { get; }
        public ItemEffect Effect { get; }
        public ItemDefinition(ItemData data)
        { Id = data.id; Name = data.displayName; Description = data.description; Amount = data.amount; Recovery = data.recovery; TargetMode = data.targetMode; Effect = data.effect; }
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
        public double EffectiveSpeed => Definition.Speed;
        public int AiActionsCompleted { get; internal set; }
        public bool IsAlive => Health > 0;
        public event Action Changed;
        public CombatantState(CombatantDefinition definition)
        { Definition = definition ?? throw new ArgumentNullException(nameof(definition)); Health = definition.MaxHealth; Mana = definition.MaxMana; }
        public int Damage(int amount)
        {
            int actual = Math.Min(Health, Math.Max(0, amount)); Health -= actual;
            if (!IsAlive) { Atb = 0; RecoveryRemaining = 0; }
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
        internal PendingCombatAction(CombatantState actor, CombatantState target, CombatAction kind, ActionDefinition action, ItemDefinition item)
        { Actor = actor; Target = target; Kind = kind; Action = action; Item = item; Recovery = item != null ? item.Recovery : action.Recovery; }
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
                // H04 adds status processing before either decision path.
                Actor = next; return;
            }
        }
        private void ReconcileDeaths()
        {
            ready.RemoveAll(e => !e.Fighter.IsAlive);
            if (!fighters.Any(f => f.Definition.Team == CombatTeam.Enemies && f.IsAlive)) Outcome = CombatOutcome.Victory;
            else if (!fighters.Any(f => f.Definition.Team == CombatTeam.Allies && f.IsAlive)) Outcome = CombatOutcome.Defeat;
            if (Outcome != CombatOutcome.Ongoing) { ready.Clear(); if (Pending == null) Actor = null; return; }
            if (Actor != null && !Actor.IsAlive && Pending == null) { Actor = null; BeginNextTurn(); }
        }
        public static int CalculateDamage(int attack, double potency, double defense) =>
            Math.Max(1, (int)Math.Round(attack * potency - defense, MidpointRounding.AwayFromZero));
        public ActionDefinition ActionFor(CombatAction command) => Actor?.Definition.Actions.FirstOrDefault(a => a.Command == command);
        public ItemDefinition Potion => items["potion"];
        public CombatAction[] AvailableCommands() => Actor == null ? new CombatAction[0] : Actor.Definition.Actions.Select(a => a.Command)
            .Concat(Actor.Definition.Team == CombatTeam.Allies ? new[] { CombatAction.Potion } : new CombatAction[0]).ToArray();
        public double RecoveryFor(CombatAction command) => command == CombatAction.Potion ? Potion.Recovery : ActionFor(command)?.Recovery ?? 0;
        public string NameFor(CombatAction command) => command == CombatAction.Potion ? Potion.Name : ActionFor(command)?.Name ?? "Acción no disponible";
        public List<CombatantState> ResolveTargets(CombatantState actor, TargetMode mode)
        {
            if (actor == null || !actor.IsAlive || !fighters.Contains(actor)) return new List<CombatantState>();
            return fighters.Where(f => f.IsAlive && (mode == TargetMode.Ally ? f.Definition.Team == actor.Definition.Team : f.Definition.Team != actor.Definition.Team)).ToList();
        }
        public List<CombatantState> TargetsFor(CombatAction command)
        {
            var action = ActionFor(command);
            if (Actor == null || (command != CombatAction.Potion && action == null)) return new List<CombatantState>();
            var targets = ResolveTargets(Actor, command == CombatAction.Potion ? Potion.TargetMode : action.TargetMode);
            if (command == CombatAction.Potion || action.Effect == ActionEffect.Heal) targets.RemoveAll(f => f.Health == f.Definition.MaxHealth);
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
            if (command == CombatAction.Potion)
            {
                if (Actor.Definition.Team != CombatTeam.Allies) return "Objeto no disponible.";
                if (Potions == 0) return "Sin unidades.";
            }
            else
            {
                var action = ActionFor(command);
                if (action == null) return "Habilidad no disponible.";
                if (action.TargetMode == TargetMode.AllEnemies || (action.Effect != ActionEffect.Damage && action.Effect != ActionEffect.Heal)) return "Efecto pendiente de implementación.";
                if (Actor.Mana < action.ManaCost) return "Maná insuficiente.";
            }
            return TargetsFor(command).Count == 0 ? "No hay objetivos válidos." : null;
        }
        public bool TryCommit(CombatAction command, CombatantState target, out string reason)
        {
            ReconcileDeaths(); reason = UnavailableReason(command);
            if (reason != null) return false;
            if (!TargetsFor(command).Contains(target)) { reason = "Objetivo inválido."; return false; }
            Pending = new PendingCombatAction(Actor, target, command, ActionFor(command), command == CombatAction.Potion ? Potion : null);
            if (command == CombatAction.Potion) inventory["potion"]--;
            else Actor.SpendMana(Pending.Action.ManaCost);
            LastEvent = Actor.Definition.Name + " prepara " + NameFor(command) + " → " + target.Definition.Name;
            return true;
        }
        public bool CommitEnemyAction()
        {
            if (Actor == null || Actor.Definition.Team != CombatTeam.Enemies || Pending != null || Outcome != CombatOutcome.Ongoing) return false;
            var targets = TargetsFor(CombatAction.Basic);
            return targets.Count > 0 && TryCommit(CombatAction.Basic, targets[random.Next(targets.Count)], out _);
        }
        public bool ApplyImpact()
        {
            if (IsPaused || Pending == null || Pending.ImpactApplied || Outcome != CombatOutcome.Ongoing) return false;
            Pending.ImpactApplied = true;
            bool healing = Pending.Item != null || Pending.Action.Effect == ActionEffect.Heal;
            Pending.EffectiveAmount = healing ? Pending.Target.Heal(Pending.Item?.Amount ?? Pending.Action.Amount) :
                Pending.Target.Damage(CalculateDamage(Pending.Actor.Definition.Attack, Pending.Action.Potency, Pending.Target.Definition.Defense));
            LastEvent = Pending.Actor.Definition.Name + ": " + NameFor(Pending.Kind) + " → " + Pending.Target.Definition.Name +
                (healing ? " +" : " −") + Pending.EffectiveAmount + " vida" + (Pending.Target.IsAlive ? "." : ". Derrotado.");
            ReconcileDeaths(); return true;
        }
        public bool FinishAction()
        {
            if (IsPaused || Pending == null || !Pending.ImpactApplied) return false;
            var actor = Pending.Actor;
            actor.Atb = 0; actor.RecoveryRemaining = actor.IsAlive ? Pending.Recovery : 0;
            if (actor.Definition.Team == CombatTeam.Enemies) actor.AiActionsCompleted++;
            // H04 adds action-end status expiration before dispatching another turn.
            Pending = null; Actor = null; CompletedActions++;
            ReconcileDeaths(); BeginNextTurn(); return true;
        }
        public static string ActionName(CombatAction command, CombatantState actor) => command == CombatAction.Potion ? "Poción" :
            actor?.Definition.Actions.FirstOrDefault(a => a.Command == command)?.Name ?? "Acción no disponible";
    }
}
