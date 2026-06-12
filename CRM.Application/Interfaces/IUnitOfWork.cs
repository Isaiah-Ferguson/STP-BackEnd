using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Participant> Participants { get; }
    IRepository<StaffMember> Staff { get; }
    IRepository<CrmProgram> Programs { get; }
    IRepository<AttendanceRecord> Attendance { get; }
    IRepository<Session> Sessions { get; }
    IRepository<CalendarEvent> CalendarEvents { get; }
    IRepository<Project> Projects { get; }
    IRepository<ProjectTask> Tasks { get; }
    IRepository<Script> Scripts { get; }

    Task<int> SaveChangesAsync();
}
