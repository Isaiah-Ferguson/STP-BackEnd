namespace CRM.Domain.Enums;

/// <summary>
/// Lifecycle of a single class meeting. A session starts <see cref="Open"/> while
/// a teacher is taking attendance and becomes <see cref="Submitted"/> once finalized,
/// after which its records are locked.
/// </summary>
public enum SessionStatus
{
    Open,
    Submitted,
}
