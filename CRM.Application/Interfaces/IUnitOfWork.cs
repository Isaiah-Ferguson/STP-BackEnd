using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

// Deliberately NOT IDisposable: the DbContext is owned by the DI container (scoped),
// so the unit of work must not dispose it.
public interface IUnitOfWork
{
    IRepository<Participant> Participants { get; }
    IRepository<StaffMember> Staff { get; }
    IRepository<CrmProgram> Programs { get; }
    IRepository<ObjectiveArea> ObjectiveAreas { get; }
    IRepository<SubSkill> SubSkills { get; }
    IRepository<Game> Games { get; }
    IRepository<GameSubGoal> GameSubGoals { get; }
    IRepository<Site> Sites { get; }
    IRepository<StarGroup> StarGroups { get; }
    IRepository<RosterAssignment> RosterAssignments { get; }
    IRepository<ParticipantArtsProfile> ParticipantArtsProfiles { get; }
    IRepository<WeeklyDataEntry> WeeklyDataEntries { get; }
    IRepository<MonthlyProgressSnapshot> MonthlyProgressSnapshots { get; }
    IRepository<WeeklyFocusSkill> WeeklyFocusSkills { get; }
    IRepository<ScoreThreshold> ScoreThresholds { get; }
    IRepository<GoalBankEntry> GoalBankEntries { get; }
    IRepository<WeeklyNoteSelection> WeeklyNoteSelections { get; }
    IRepository<MonthlySummary> MonthlySummaries { get; }
    IRepository<GameIdea> GameIdeas { get; }
    IRepository<AgeModification> AgeModifications { get; }
    IRepository<PerStarPlan> PerStarPlans { get; }
    IRepository<CalendarTheme> CalendarThemes { get; }
    IRepository<KeyArtsDate> KeyArtsDates { get; }
    IRepository<AttendanceRecord> Attendance { get; }
    IRepository<AttendanceNote> AttendanceNotes { get; }
    IRepository<Session> Sessions { get; }
    IRepository<CalendarEvent> CalendarEvents { get; }
    IRepository<Project> Projects { get; }
    IRepository<ProjectTask> Tasks { get; }
    IRepository<Script> Scripts { get; }
    IRepository<OnboardingItem> OnboardingItems { get; }
    IRepository<User> Users { get; }

    // StaffProgramAssignment has a composite PK and does not extend BaseEntity,
    // so it is exposed via dedicated methods rather than a generic repository.
    Task<IReadOnlyList<StaffProgramAssignment>> GetStaffProgramAssignmentsAsync();
    Task AddStaffProgramAssignmentAsync(StaffProgramAssignment assignment);

    Task<int> SaveChangesAsync();
}
