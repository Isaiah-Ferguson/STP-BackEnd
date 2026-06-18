namespace CRM.Domain.Enums;

/// <summary>
/// The days of the week a program regularly meets. A [Flags] enum so a single
/// program can meet on several days (e.g. Monday | Wednesday | Friday), stored
/// as an int and queryable with <see cref="System.Enum.HasFlag"/>.
/// </summary>
[Flags]
public enum MeetingDays
{
    None = 0,
    Sunday = 1,
    Monday = 2,
    Tuesday = 4,
    Wednesday = 8,
    Thursday = 16,
    Friday = 32,
    Saturday = 64,
}
