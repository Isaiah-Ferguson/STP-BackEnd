namespace CRM.Domain.Enums;

/// <summary>
/// The progress tier(s) a game targets. A [Flags] enum (modelled like
/// <see cref="MeetingDays"/>) so a single game can span several tiers — e.g.
/// "Novice, Intermediate" — and "All Tiers" is the combination of all three.
/// Distinct from <see cref="ProgressLevel"/>, which is a single scored value and
/// cannot be combined. Stored as an int; queryable with <see cref="System.Enum.HasFlag"/>
/// or a bitmask test.
/// </summary>
[Flags]
public enum GameTier
{
    None = 0,
    Novice = 1,
    Intermediate = 2,
    Expert = 4,
    All = Novice | Intermediate | Expert,
}
