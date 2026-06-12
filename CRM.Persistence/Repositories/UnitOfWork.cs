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
        Attendance = new GenericRepository<AttendanceRecord>(db);
        AttendanceNotes = new GenericRepository<AttendanceNote>(db);
        Sessions = new GenericRepository<Session>(db);
        CalendarEvents = new GenericRepository<CalendarEvent>(db);
        Projects = new GenericRepository<Project>(db);
        Tasks = new GenericRepository<ProjectTask>(db);
        Scripts = new GenericRepository<Script>(db);
        OnboardingItems = new GenericRepository<OnboardingItem>(db);
        Users = new GenericRepository<User>(db);
    }

    public IRepository<Participant> Participants { get; }
    public IRepository<StaffMember> Staff { get; }
    public IRepository<CrmProgram> Programs { get; }
    public IRepository<AttendanceRecord> Attendance { get; }
    public IRepository<AttendanceNote> AttendanceNotes { get; }
    public IRepository<Session> Sessions { get; }
    public IRepository<CalendarEvent> CalendarEvents { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<ProjectTask> Tasks { get; }
    public IRepository<Script> Scripts { get; }
    public IRepository<OnboardingItem> OnboardingItems { get; }
    public IRepository<User> Users { get; }

    public async Task<IReadOnlyList<StaffProgramAssignment>> GetStaffProgramAssignmentsAsync() =>
        await _db.Set<StaffProgramAssignment>().AsNoTracking().ToListAsync();

    public async Task AddStaffProgramAssignmentAsync(StaffProgramAssignment assignment) =>
        await _db.Set<StaffProgramAssignment>().AddAsync(assignment);

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

    public void Dispose() => _db.Dispose();
}
