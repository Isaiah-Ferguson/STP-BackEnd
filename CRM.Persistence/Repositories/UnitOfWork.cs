using CRM.Application.Interfaces;
using CRM.Domain.Entities;

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
        Sessions = new GenericRepository<Session>(db);
        CalendarEvents = new GenericRepository<CalendarEvent>(db);
        Projects = new GenericRepository<Project>(db);
        Tasks = new GenericRepository<ProjectTask>(db);
        Scripts = new GenericRepository<Script>(db);
    }

    public IRepository<Participant> Participants { get; }
    public IRepository<StaffMember> Staff { get; }
    public IRepository<CrmProgram> Programs { get; }
    public IRepository<AttendanceRecord> Attendance { get; }
    public IRepository<Session> Sessions { get; }
    public IRepository<CalendarEvent> CalendarEvents { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<ProjectTask> Tasks { get; }
    public IRepository<Script> Scripts { get; }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

    public void Dispose() => _db.Dispose();
}
