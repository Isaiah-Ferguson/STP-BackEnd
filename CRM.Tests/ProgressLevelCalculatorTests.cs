using CRM.Application.Services;
using CRM.Domain.Enums;

namespace CRM.Tests;

public class ProgressLevelCalculatorTests
{
    // Typical threshold table: Novice is the floor, Intermediate from 1.5, Expert from 2.5.
    private static readonly IReadOnlyList<(ProgressLevel Level, double MinAverage)> Thresholds =
    [
        (ProgressLevel.Novice, 0.0),
        (ProgressLevel.Intermediate, 1.5),
        (ProgressLevel.Expert, 2.5),
    ];

    [Theory]
    [InlineData(DataScore.Refusal, 0)]
    [InlineData(DataScore.FullPrompts, 1)]
    [InlineData(DataScore.MinimalPrompts, 2)]
    [InlineData(DataScore.Independent, 3)]
    public void Points_MapsScoredValues(DataScore score, int expected)
    {
        Assert.Equal(expected, ProgressLevelCalculator.Points(score));
    }

    [Fact]
    public void Points_NotApplicable_IsNull()
    {
        Assert.Null(ProgressLevelCalculator.Points(DataScore.NotApplicable));
    }

    [Fact]
    public void Derive_NoScoredWeeks_IsNotApplicable()
    {
        var result = ProgressLevelCalculator.Derive(
            [DataScore.NotApplicable, DataScore.NotApplicable], Thresholds);

        Assert.Equal(ProgressLevel.NotApplicable, result.Level);
        Assert.Equal(0, result.ScoredWeekCount);
    }

    [Fact]
    public void Derive_ExcludesNotApplicableFromAverage()
    {
        // One Independent (3) + one N/A → average 3.0, not 1.5.
        var result = ProgressLevelCalculator.Derive(
            [DataScore.Independent, DataScore.NotApplicable], Thresholds);

        Assert.Equal(3.0, result.Average);
        Assert.Equal(1, result.ScoredWeekCount);
        Assert.Equal(ProgressLevel.Expert, result.Level);
    }

    [Fact]
    public void Derive_SingleIndependentWeek_NotPenalisedVersusFourWeeks()
    {
        var oneWeek = ProgressLevelCalculator.Derive([DataScore.Independent], Thresholds);
        var fourWeeks = ProgressLevelCalculator.Derive(
            [DataScore.Independent, DataScore.Independent, DataScore.Independent, DataScore.Independent],
            Thresholds);

        Assert.Equal(fourWeeks.Level, oneWeek.Level);
    }

    [Fact]
    public void Derive_AllRefusals_IsNovice()
    {
        var result = ProgressLevelCalculator.Derive(
            [DataScore.Refusal, DataScore.Refusal], Thresholds);

        Assert.Equal(ProgressLevel.Novice, result.Level);
        Assert.Equal(0, result.SummedScore);
    }

    [Fact]
    public void Derive_AverageExactlyAtCutoff_MeetsTheLevel()
    {
        // FullPrompts (1) + MinimalPrompts (2) → average 1.5 → exactly the Intermediate cutoff.
        var result = ProgressLevelCalculator.Derive(
            [DataScore.FullPrompts, DataScore.MinimalPrompts], Thresholds);

        Assert.Equal(ProgressLevel.Intermediate, result.Level);
    }

    [Fact]
    public void Derive_PicksHighestQualifyingLevel()
    {
        // All Independent → 3.0 qualifies for every level; Expert (highest cutoff) must win.
        var result = ProgressLevelCalculator.Derive(
            [DataScore.Independent, DataScore.Independent], Thresholds);

        Assert.Equal(ProgressLevel.Expert, result.Level);
    }

    [Fact]
    public void Derive_EmptyThresholds_FallsBackToNovice()
    {
        var result = ProgressLevelCalculator.Derive([DataScore.Independent], []);

        Assert.Equal(ProgressLevel.Novice, result.Level);
    }
}
