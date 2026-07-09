using CRM.Application.Services;
using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Tests;

public class AttendanceStatsTests
{
    private static AttendanceRecord Record(AttendanceStatus status, Guid? participantId = null) =>
        new() { ParticipantId = participantId ?? Guid.NewGuid(), SessionId = Guid.NewGuid(), Status = status };

    [Fact]
    public void PercentFor_NoRecords_ReturnsZero()
    {
        Assert.Equal(0, AttendanceStats.PercentFor([]));
    }

    [Fact]
    public void PercentFor_OnlyUnmarked_ReturnsZero()
    {
        var records = new[] { Record(AttendanceStatus.Unmarked), Record(AttendanceStatus.Unmarked) };
        Assert.Equal(0, AttendanceStats.PercentFor(records));
    }

    [Fact]
    public void PercentFor_AllPresent_Returns100()
    {
        var records = new[] { Record(AttendanceStatus.Present), Record(AttendanceStatus.Present) };
        Assert.Equal(100, AttendanceStats.PercentFor(records));
    }

    [Fact]
    public void PercentFor_AllAbsent_ReturnsZero()
    {
        var records = new[] { Record(AttendanceStatus.Absent), Record(AttendanceStatus.Absent) };
        Assert.Equal(0, AttendanceStats.PercentFor(records));
    }

    [Fact]
    public void PercentFor_MixedWithUnmarked_IgnoresUnmarked()
    {
        // 3 Present, 1 Absent, 2 Unmarked → 3/4 = 75%; Unmarked must not dilute the rate.
        var records = new[]
        {
            Record(AttendanceStatus.Present),
            Record(AttendanceStatus.Present),
            Record(AttendanceStatus.Present),
            Record(AttendanceStatus.Absent),
            Record(AttendanceStatus.Unmarked),
            Record(AttendanceStatus.Unmarked),
        };
        Assert.Equal(75, AttendanceStats.PercentFor(records));
    }

    [Fact]
    public void PercentFor_Rounds()
    {
        // 2 Present, 1 Absent → 66.67% → 67.
        var records = new[]
        {
            Record(AttendanceStatus.Present),
            Record(AttendanceStatus.Present),
            Record(AttendanceStatus.Absent),
        };
        Assert.Equal(67, AttendanceStats.PercentFor(records));
    }

    [Fact]
    public void PercentByParticipant_GroupsPerParticipant()
    {
        var alice = Guid.NewGuid();
        var bob = Guid.NewGuid();
        var records = new[]
        {
            Record(AttendanceStatus.Present, alice),
            Record(AttendanceStatus.Absent, alice),
            Record(AttendanceStatus.Present, bob),
        };

        var map = AttendanceStats.PercentByParticipant(records);

        Assert.Equal(50, map[alice]);
        Assert.Equal(100, map[bob]);
    }

    [Fact]
    public void PercentByParticipant_EmptyInput_EmptyMap()
    {
        Assert.Empty(AttendanceStats.PercentByParticipant([]));
    }
}
