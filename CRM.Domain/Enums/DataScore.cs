namespace CRM.Domain.Enums;

/// <summary>
/// What a Star DID on a focus skill in a given week. Scored 0–3, or N/A when the skill
/// wasn't targeted that week. The 0–3 values double as the points used in month-end
/// derivation; N/A weeks are excluded from the average (see ProgressLevelCalculator).
/// </summary>
public enum DataScore
{
    Refusal = 0,
    FullPrompts = 1,
    MinimalPrompts = 2,
    Independent = 3,
    NotApplicable = 9,
}
