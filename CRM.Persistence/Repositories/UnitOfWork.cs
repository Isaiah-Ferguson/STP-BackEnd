using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Participants = new GenericRepository<Participant>(db);
        Staff = new GenericRepository<StaffMember>(db);
        Programs = new GenericRepository<CrmProgram>(db);
        ObjectiveAreas = new GenericRepository<ObjectiveArea>(db);
        SubSkills = new GenericRepository<SubSkill>(db);
        Games = new GenericRepository<Game>(db);
        GameSubGoals = new GenericRepository<GameSubGoal>(db);
        Sites = new GenericRepository<Site>(db);
        StarGroups = new GenericRepository<StarGroup>(db);
        RosterAssignments = new GenericRepository<RosterAssignment>(db);
        ParticipantArtsProfiles = new GenericRepository<ParticipantArtsProfile>(db);
        WeeklyDataEntries = new GenericRepository<WeeklyDataEntry>(db);
        MonthlyProgressSnapshots = new GenericRepository<MonthlyProgressSnapshot>(db);
        WeeklyFocusSkills = new GenericRepository<WeeklyFocusSkill>(db);
        ScoreThresholds = new GenericRepository<ScoreThreshold>(db);
        GoalBankEntries = new GenericRepository<GoalBankEntry>(db);
        WeeklyNoteSelections = new GenericRepository<WeeklyNoteSelection>(db);
        MonthlySummaries = new GenericRepository<MonthlySummary>(db);
        GameIdeas = new GenericRepository<GameIdea>(db);
        AgeModifications = new GenericRepository<AgeModification>(db);
        PerStarPlans = new GenericRepository<PerStarPlan>(db);
        CalendarThemes = new GenericRepository<CalendarTheme>(db);
        KeyArtsDates = new GenericRepository<KeyArtsDate>(db);
        Attendance = new GenericRepository<AttendanceRecord>(db);
        AttendanceNotes = new GenericRepository<AttendanceNote>(db);
        Sessions = new GenericRepository<Session>(db);
        CalendarEvents = new GenericRepository<CalendarEvent>(db);
        Projects = new GenericRepository<Project>(db);
        Tasks = new GenericRepository<ProjectTask>(db);
        Scripts = new GenericRepository<Script>(db);
        OnboardingItems = new GenericRepository<OnboardingItem>(db);
        Users = new GenericRepository<User>(db);
        RefreshTokens = new GenericRepository<RefreshToken>(db);
    }

    public IRepository<Participant> Participants { get; }
    public IRepository<StaffMember> Staff { get; }
    public IRepository<CrmProgram> Programs { get; }
    public IRepository<ObjectiveArea> ObjectiveAreas { get; }
    public IRepository<SubSkill> SubSkills { get; }
    public IRepository<Game> Games { get; }
    public IRepository<GameSubGoal> GameSubGoals { get; }
    public IRepository<Site> Sites { get; }
    public IRepository<StarGroup> StarGroups { get; }
    public IRepository<RosterAssignment> RosterAssignments { get; }
    public IRepository<ParticipantArtsProfile> ParticipantArtsProfiles { get; }
    public IRepository<WeeklyDataEntry> WeeklyDataEntries { get; }
    public IRepository<MonthlyProgressSnapshot> MonthlyProgressSnapshots { get; }
    public IRepository<WeeklyFocusSkill> WeeklyFocusSkills { get; }
    public IRepository<ScoreThreshold> ScoreThresholds { get; }
    public IRepository<GoalBankEntry> GoalBankEntries { get; }
    public IRepository<WeeklyNoteSelection> WeeklyNoteSelections { get; }
    public IRepository<MonthlySummary> MonthlySummaries { get; }
    public IRepository<GameIdea> GameIdeas { get; }
    public IRepository<AgeModification> AgeModifications { get; }
    public IRepository<PerStarPlan> PerStarPlans { get; }
    public IRepository<CalendarTheme> CalendarThemes { get; }
    public IRepository<KeyArtsDate> KeyArtsDates { get; }
    public IRepository<AttendanceRecord> Attendance { get; }
    public IRepository<AttendanceNote> AttendanceNotes { get; }
    public IRepository<Session> Sessions { get; }
    public IRepository<CalendarEvent> CalendarEvents { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<ProjectTask> Tasks { get; }
    public IRepository<Script> Scripts { get; }
    public IRepository<OnboardingItem> OnboardingItems { get; }
    public IRepository<User> Users { get; }
    public IRepository<RefreshToken> RefreshTokens { get; }

    public async Task<IReadOnlyList<StaffProgramAssignment>> GetStaffProgramAssignmentsAsync() =>
        await _db.Set<StaffProgramAssignment>().AsNoTracking().ToListAsync();

    public async Task AddStaffProgramAssignmentAsync(StaffProgramAssignment assignment) =>
        await _db.Set<StaffProgramAssignment>().AddAsync(assignment);

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
