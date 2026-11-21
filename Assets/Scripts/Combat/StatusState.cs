using System;

namespace UltimaGuardia
{
    // Immutable authoring snapshot. Active charges belong only to StatusState.
    public sealed class EffectDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public StatusKind Kind { get; }
        public EffectTrigger Trigger { get; }
        public int Duration { get; }
        public int FixedDamage { get; }
        public double Multiplier { get; }
        public EffectDefinition(EffectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Id = data.id; Name = data.displayName; Kind = data.kind; Trigger = data.trigger;
            Duration = data.duration; FixedDamage = data.fixedDamage; Multiplier = data.multiplier;
        }
    }

    public sealed class StatusState
    {
        public EffectDefinition Definition { get; }
        public int Remaining { get; internal set; }
        public long ApplicationId { get; }
        internal StatusState(EffectDefinition definition, long applicationId)
        { Definition = definition; Remaining = definition.Duration; ApplicationId = applicationId; }
    }
}
