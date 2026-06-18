using CRM.Application.DTOs.Attendance;
using CRM.Application.DTOs.Calendar;
using CRM.Application.DTOs.Participants;
using CRM.Application.DTOs.Programs;
using CRM.Application.DTOs.Staff;
using CRM.Application.DTOs.Tasks;

namespace CRM.Application.DTOs.Dashboard;

/// <summary>
/// Everything the dashboard renders, in one response — so the page makes a single
/// request instead of seven. Today's roster is read-only (no lazy session creation).
/// </summary>
public class DashboardDto
{
    public List<ParticipantSummaryDto> Participants { get; set; } = new();
    public List<AttendanceRosterEntryDto> TodayRoster { get; set; } = new();
    public List<ProjectDto> Projects { get; set; } = new();
    public List<StaffSummaryDto> Staff { get; set; } = new();
    public List<ProgramSummaryDto> Programs { get; set; } = new();
    public List<CalendarEventDto> Events { get; set; } = new();
}
