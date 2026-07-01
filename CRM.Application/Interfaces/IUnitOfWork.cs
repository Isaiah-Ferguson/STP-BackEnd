using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Participant> Participants { get; }
    IRepository<StaffMember> Staff { get; }
    IRepository<CrmProgram> Programs { get; }
    IRepository<ObjectiveArea> ObjectiveAreas { get; }
    IRepository<SubSkill> SubSkills { get; }
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
