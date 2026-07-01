using CRM.Application.DTOs.Reports;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class ReportsService : IReportsService
{
    private readonly IUnitOfWork _uow;

    public ReportsService(IUnitOfWork uow) => _uow = uow;

    public async Task<ReportsDto> GetAsync()
    {
        // Each table loaded once, aggregated in memory.
        var participants = await _uow.Participants.GetAllAsync();
        var programs = await _uow.Programs.GetAllAsync();
        var staff = await _uow.Staff.GetAllAsync();
        var sessions = await _uow.Sessions.GetAllAsync();
        var records = await _uow.Attendance.GetAllAsync();
        var tasks = await _uow.Tasks.GetAllAsync();

        var ptsByProgram = participants
            .GroupBy(p => p.ProgramId)
            .ToDictionary(g => g.Key, g => g.ToList());
        var sessionCountByProgram = sessions
            .GroupBy(s => s.ProgramId)
            .ToDictionary(g => g.Key, g => g.Count());

        var programReports = programs
            .Select(p =>
            {
                var pts = ptsByProgram.GetValueOrDefault(p.Id, new());
                return new ProgramReportDto
                {
                    Slug = p.Slug,
                    Name = p.Name,
                    Enrolled = pts.Count(x => x.Status == ParticipantStatus.Active),
                    AttendancePct = pts.Count > 0 ? (int)Math.Round(pts.Average(x => x.AttendancePct)) : 0,
                    Sessions = sessionCountByProgram.GetValueOrDefault(p.Id, 0),
                };
            })
            .OrderBy(p => p.Name)
            .ToList();

        var present = records.Count(r => r.Status == AttendanceStatus.Present);
        var absent = records.Count(r => r.Status == AttendanceStatus.Absent);
        var unmarked = records.Count(r => r.Status == AttendanceStatus.Unmarked);

        var totals = new ReportTotalsDto
        {
            TotalParticipants = participants.Count,
            ActiveParticipants = participants.Count(p => p.Status == ParticipantStatus.Active),
            Prospective = participants.Count(p => p.Status == ParticipantStatus.Prospective),
            Attention = participants.Count(p => p.Status == ParticipantStatus.Attention),
            Former = participants.Count(p => p.Status == ParticipantStatus.Former),
            Programs = programs.Count,
            Staff = staff.Count,
            FullyOnboardedStaff = staff.Count(s => s.OnboardingProgressPct == 100),
            AvgAttendancePct = participants.Count > 0 ? (int)Math.Round(participants.Average(p => p.AttendancePct)) : 0,
            OpenTasks = tasks.Count(t => t.Status != Domain.Enums.TaskStatus.Done),
            OverdueTasks = tasks.Count(t => t.IsOverdue || t.Status == Domain.Enums.TaskStatus.Overdue),
        };

        var staffOnboarding = staff
            .OrderBy(s => s.OnboardingProgressPct)
            .ThenBy(s => s.FullName)
            .Select(s => new StaffOnboardingReportDto { Name = s.FullName, Pct = s.OnboardingProgressPct })
            .ToList();

        return new ReportsDto
        {
            Totals = totals,
            Programs = programReports,
            StaffOnboarding = staffOnboarding,
            Attendance = new AttendanceSummaryDto
            {
                Sessions = sessions.Count,
                Present = present,
                Absent = absent,
                Unmarked = unmarked,
                PresentRatePct = (present + absent) > 0 ? (int)Math.Round(100.0 * present / (present + absent)) : 0,
            },
        };
    }
}
