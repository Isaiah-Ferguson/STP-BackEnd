using CRM.Domain.Enums;

namespace CRM.Application.Services;

/// <summary>
/// Derives a suggested month-end Progress Level from a Star's weekly Data scores on one
/// skill, using the AVERAGE of the weeks the skill was actually scored (N/A weeks
/// excluded). Averaging normalises for the variable number of focus weeks per skill, so a
/// skill scored once at "Independent" isn't penalised versus one scored four times.
/// </summary>
public static class ProgressLevelCalculator
{
    /// <summary>Points a scored week contributes (0–3); null for N/A (excluded from the average).</summary>
    public static int? Points(DataScore s) => s switch
    {
        DataScore.Refusal => 0,
        DataScore.FullPrompts => 1,
        DataScore.MinimalPrompts => 2,
        DataScore.Independent => 3,
        _ => null, // NotApplicable
    };

    public readonly record struct Result(ProgressLevel Level, int SummedScore, int ScoredWeekCount, double Average);

    /// <summary>
    /// Thresholds are (Level, MinAverage) pairs — a level applies when the average is at
    /// least its minimum. With no scored weeks the result is <see cref="ProgressLevel.NotApplicable"/>.
    /// </summary>
    public static Result Derive(IEnumerable<DataScore> scores, IReadOnlyList<(ProgressLevel Level, double MinAverage)> thresholds)
    {
        var points = scores.Select(Points).Where(p => p.HasValue).Select(p => p!.Value).ToList();
        if (points.Count == 0)
            return new Result(ProgressLevel.NotApplicable, 0, 0, 0);

        var sum = points.Sum();
        var avg = (double)sum / points.Count;

        // Highest-cutoff level whose minimum the average meets. Novice (0.0) is the floor.
        var level = thresholds
            .Where(t => avg >= t.MinAverage)
            .OrderByDescending(t => t.MinAverage)
            .Select(t => (ProgressLevel?)t.Level)
            .FirstOrDefault() ?? ProgressLevel.Novice;

        return new Result(level, sum, points.Count, avg);
    }
}
