using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimaGuardia
{
    public enum CombatTeam { Allies, Enemies }
    public enum CombatAction { Basic, HeavyStrike, Potion }
    public enum TargetMode { Enemy, Ally, AllEnemies }
    public enum CombatOutcome { Ongoing, Victory, Defeat }

    // H01 configuration is immutable C# data. H02 moves it into editable assets.
    public sealed class CombatantDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public CombatTeam Team { get; }
        public int MaxHealth { get; }
        public int MaxMana { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int Speed { get; }

        public CombatantDefinition(string id, string name, CombatTeam team,
            int health, int mana, int attack, int defense, int speed)
        {
            Id = id; Name = name; Team = team; MaxHealth = health;
            MaxMana = mana; Attack = attack; Defense = defense; Speed = speed;
        }
    }

    public sealed class CombatantState
    {
        public CombatantDefinition Definition { get; }
        public int Health { get; private set; }
        public int Mana { get; private set; }
        public bool IsAlive => Health > 0;
        public event Action Changed;

        public CombatantState(CombatantDefinition definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            Health = definition.MaxHealth; Mana = definition.MaxMana;
        }

        public int Damage(int amount)
        {
            int actual = Math.Min(Health, Math.Max(0, amount));
            Health -= actual;
            if (actual > 0) Changed?.Invoke();
            return actual;
        }

        public int Heal(int amount)
        {
            if (!IsAlive) return 0;
            int actual = Math.Min(Definition.MaxHealth - Health, Math.Max(0, amount));
            Health += actual;
            if (actual > 0) Changed?.Invoke();
            return actual;
        }

        public int RestoreMana(int amount)
        {
            if (!IsAlive) return 0;
            int actual = Math.Min(Definition.MaxMana - Mana, Math.Max(0, amount));
            Mana += actual;
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
        public bool ImpactApplied { get; internal set; }
        public int EffectiveAmount { get; internal set; }
        internal PendingCombatAction(CombatantState actor, CombatantState target, CombatAction kind)
        { Actor = actor; Target = target; Kind = kind; }
    }

    // Single owner of turn, validation, spending and impact. No Unity timing in these rules.
    public sealed class CombatSession
    {
        private readonly CombatantState[] fighters;
        private readonly Random random;
        private int actorIndex;
        public IReadOnlyList<CombatantState> Fighters { get; }
        public CombatantState Actor => fighters[actorIndex];
        public PendingCombatAction Pending { get; private set; }
        public CombatOutcome Outcome { get; private set; }
        public int Potions { get; private set; } = 3;
        public int CompletedActions { get; private set; }
        public string LastEvent { get; private set; } = "Defendé la entrada. Derrotá al ogro y al chamán.";

        public CombatSession(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
            fighters = new[]
            {
                new CombatantState(new CombatantDefinition("warrior", "Guerrero", CombatTeam.Allies, 160, 30, 28, 8, 22)),
                new CombatantState(new CombatantDefinition("mage", "Maga", CombatTeam.Allies, 110, 45, 24, 4, 28)),
                new CombatantState(new CombatantDefinition("ogre", "Ogro", CombatTeam.Enemies, 240, 24, 28, 6, 16)),
                new CombatantState(new CombatantDefinition("shaman", "Chamán", CombatTeam.Enemies, 130, 36, 18, 4, 20))
            };
            Fighters = Array.AsReadOnly(fighters);
        }

        public static int CalculateDamage(int attack, double potency, double defense) =>
            Math.Max(1, (int)Math.Round(attack * potency - defense, MidpointRounding.AwayFromZero));

        public List<CombatantState> ResolveTargets(CombatantState actor, TargetMode mode)
        {
            if (actor == null || !actor.IsAlive || !fighters.Contains(actor)) return new List<CombatantState>();
            return fighters.Where(f => f.IsAlive && (mode == TargetMode.Ally
                ? f.Definition.Team == actor.Definition.Team : f.Definition.Team != actor.Definition.Team)).ToList();
        }

        public List<CombatantState> TargetsFor(CombatAction action)
        {
            var targets = ResolveTargets(Actor, action == CombatAction.Potion ? TargetMode.Ally : TargetMode.Enemy);
            if (action == CombatAction.Potion) targets.RemoveAll(f => f.Health == f.Definition.MaxHealth);
            return targets;
        }

        public string UnavailableReason(CombatAction action)
        {
            if (Outcome != CombatOutcome.Ongoing) return "El combate terminó.";
            if (Pending != null) return "Acción en ejecución.";
            if (!Actor.IsAlive) return "El participante está derrotado.";
            if (!Enum.IsDefined(typeof(CombatAction), action)) return "Acción desconocida.";
            if (action == CombatAction.HeavyStrike && Actor.Definition.Id != "warrior") return "Habilidad no disponible.";
            if (action == CombatAction.Potion && Actor.Definition.Team != CombatTeam.Allies) return "Objeto no disponible.";
            if (action == CombatAction.HeavyStrike && Actor.Mana < 8) return "Maná insuficiente.";
            if (action == CombatAction.Potion && Potions == 0) return "Sin unidades.";
            if (TargetsFor(action).Count == 0) return "No hay objetivos válidos.";
            return null;
        }

        public bool TryCommit(CombatAction action, CombatantState target, out string reason)
        {
            reason = UnavailableReason(action);
            if (reason != null) return false;
            if (!TargetsFor(action).Contains(target)) { reason = "Objetivo inválido."; return false; }
            // Lock before publishing resource-change events; reentrant commands cannot spend twice.
            Pending = new PendingCombatAction(Actor, target, action);
            if (action == CombatAction.HeavyStrike) Actor.SpendMana(8);
            if (action == CombatAction.Potion) Potions--;
            LastEvent = Actor.Definition.Name + " prepara " + ActionName(action, Actor) + " → " + target.Definition.Name;
            return true;
        }

        public bool CommitEnemyAction()
        {
            if (Actor.Definition.Team != CombatTeam.Enemies || Pending != null || Outcome != CombatOutcome.Ongoing) return false;
            var targets = TargetsFor(CombatAction.Basic);
            if (targets.Count == 0) { CheckOutcome(); return false; }
            return TryCommit(CombatAction.Basic, targets[random.Next(targets.Count)], out _);
        }

        public bool ApplyImpact()
        {
            if (Pending == null || Pending.ImpactApplied || Outcome != CombatOutcome.Ongoing) return false;
            Pending.ImpactApplied = true;
            if (Pending.Kind == CombatAction.Potion)
                Pending.EffectiveAmount = Pending.Target.Heal(50);
            else
                Pending.EffectiveAmount = Pending.Target.Damage(CalculateDamage(Pending.Actor.Definition.Attack,
                    Pending.Kind == CombatAction.HeavyStrike ? 1.8 : 1, Pending.Target.Definition.Defense));
            LastEvent = Pending.Actor.Definition.Name + ": " + ActionName(Pending.Kind, Pending.Actor) +
                " → " + Pending.Target.Definition.Name + (Pending.Kind == CombatAction.Potion ? " +" : " −") +
                Pending.EffectiveAmount + " vida" + (Pending.Target.IsAlive ? "." : ". Derrotado.");
            CheckOutcome();
            return true;
        }

        public bool FinishAction()
        {
            if (Pending == null || !Pending.ImpactApplied) return false;
            Pending = null;
            CompletedActions++;
            if (Outcome == CombatOutcome.Ongoing)
                for (int i = 0; i < fighters.Length; i++)
                {
                    actorIndex = (actorIndex + 1) % fighters.Length;
                    if (Actor.IsAlive) break;
                }
            return true;
        }

        private void CheckOutcome()
        {
            if (!fighters.Any(f => f.Definition.Team == CombatTeam.Enemies && f.IsAlive)) Outcome = CombatOutcome.Victory;
            else if (!fighters.Any(f => f.Definition.Team == CombatTeam.Allies && f.IsAlive)) Outcome = CombatOutcome.Defeat;
        }

        public static string ActionName(CombatAction action, CombatantState actor)
        {
            if (action == CombatAction.HeavyStrike) return "Golpe pesado";
            if (action == CombatAction.Potion) return "Poción";
            switch (actor.Definition.Id)
            {
                case "mage": return "Proyectil mágico";
                case "ogre": return "Golpe";
                case "shaman": return "Descarga";
                default: return "Ataque básico";
            }
        }
    }
}
