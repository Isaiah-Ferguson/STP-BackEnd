using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.API;

public static class DataSeeder
{
    /// <summary>
    /// Seeds local-development logins (all with password <c>ChangeMe!123</c>):
    /// an admin account, plus teacher accounts linked to their staff records so the
    /// per-teacher program scoping on the attendance page can be exercised.
    /// Each account is created only if its email doesn't already exist.
    /// </summary>
    public static async Task SeedAdminUserAsync(AppDbContext db, IPasswordHasher hasher)
    {
        // Admin login — not linked to a staff record (admins need not be staff).
        if (!await db.Users.AnyAsync(u => u.Email == "admin@shiningstars.org"))
        {
            var (hash, salt) = hasher.HashPassword("ChangeMe!123");
            db.Users.Add(new User
            {
                Email = "admin@shiningstars.org",
                FullName = "Site Administrator",
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = UserRole.Admin,
                IsActive = true,
            });
            await db.SaveChangesAsync();
        }

        // Teacher logins linked to staff. Devon teaches MJC + Manteca; Maya teaches Pathways.
        var staffLogins = new[]
        {
            (Email: "devon@shiningstars.org", FullName: "Devon P.", Initials: "DP"),
            (Email: "maya@shiningstars.org",  FullName: "Maya L.",  Initials: "ML"),
        };

        foreach (var login in staffLogins)
        {
            if (await db.Users.AnyAsync(u => u.Email == login.Email)) continue;

            var staff = await db.Staff.FirstOrDefaultAsync(s => s.Initials == login.Initials);
            if (staff is null) continue; // staff not seeded — skip

            var (hash, salt) = hasher.HashPassword("ChangeMe!123");
            db.Users.Add(new User
            {
                Email = login.Email,
                FullName = login.FullName,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = UserRole.Staff,
                StaffMemberId = staff.Id,
                IsActive = true,
            });
        }
        await db.SaveChangesAsync();
    }

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Programs.AnyAsync()) return;

        // ── Programs ──────────────────────────────────────────────────────────

        var mjc = new CrmProgram
        {
            Name = "MJC", Slug = "mjc", ColorHex = "#378add",
            SessionSchedule = "Mon / Wed / Fri", DefaultLocation = "FH 152",
            MeetingDays = MeetingDays.Monday | MeetingDays.Wednesday | MeetingDays.Friday,
            StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(11, 30),
        };
        var pathways = new CrmProgram
        {
            Name = "Pathways", Slug = "pathways", ColorHex = "#1d9e75",
            SessionSchedule = "Tue / Thu", DefaultLocation = "Room C",
            MeetingDays = MeetingDays.Tuesday | MeetingDays.Thursday,
            StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 30),
        };
        var manteca = new CrmProgram
        {
            Name = "Manteca PT", Slug = "manteca", ColorHex = "#d85a30",
            SessionSchedule = "Mon / Thu", DefaultLocation = "Gym B",
            MeetingDays = MeetingDays.Monday | MeetingDays.Thursday,
            StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(15, 0),
        };
        db.Programs.AddRange(mjc, pathways, manteca);

        // ── Staff ─────────────────────────────────────────────────────────────

        var rachel = new StaffMember { FullName = "Rachel M.", Initials = "RM", Role = StaffRole.Coordinator, StartDate = new DateTime(2022, 1, 15), OnboardingProgressPct = 100 };
        var devon  = new StaffMember { FullName = "Devon P.",  Initials = "DP", Role = StaffRole.Teacher,     StartDate = new DateTime(2022, 3, 1),  OnboardingProgressPct = 100 };
        var joss   = new StaffMember { FullName = "Joss K.",   Initials = "JK", Role = StaffRole.Coordinator, StartDate = new DateTime(2024, 8, 15), OnboardingProgressPct = 65  };
        var maya   = new StaffMember { FullName = "Maya L.",   Initials = "ML", Role = StaffRole.Teacher,     StartDate = new DateTime(2023, 9, 1),  OnboardingProgressPct = 100 };
        var nina   = new StaffMember { FullName = "Nina S.",   Initials = "NS", Role = StaffRole.Teacher,     StartDate = new DateTime(2023, 1, 10), OnboardingProgressPct = 100 };
        var alex   = new StaffMember { FullName = "Alex T.",   Initials = "AT", Role = StaffRole.Coordinator, StartDate = new DateTime(2023, 6, 1),  OnboardingProgressPct = 100 };
        db.Staff.AddRange(rachel, devon, joss, maya, nina, alex);

        // ── Participants ──────────────────────────────────────────────────────

        var mjcPts = new[]
        {
            new Participant { FullName = "Kezia Morales", Initials = "KM", ProgramId = mjc.Id,      Status = ParticipantStatus.Active,      AttendancePct = 95, StartDate = new DateTime(2023, 9, 1),  ServiceCoordinator = "D. Kwan" },
            new Participant { FullName = "Marcus T.",     Initials = "MT", ProgramId = mjc.Id,      Status = ParticipantStatus.Attention,   AttendancePct = 62, StartDate = new DateTime(2022, 3, 1),  ServiceCoordinator = "D. Kwan" },
            new Participant { FullName = "Sofia Reyes",   Initials = "SR", ProgramId = mjc.Id,      Status = ParticipantStatus.Attention,   AttendancePct = 0,  StartDate = new DateTime(2026, 5, 1),  ServiceCoordinator = "D. Kwan" },
            new Participant { FullName = "Eli Hart",      Initials = "EH", ProgramId = mjc.Id,      Status = ParticipantStatus.Active,      AttendancePct = 90, StartDate = new DateTime(2023, 8, 1),  ServiceCoordinator = "D. Kwan" },
            new Participant { FullName = "Aaron Torres",  Initials = "AT", ProgramId = mjc.Id,      Status = ParticipantStatus.Prospective, AttendancePct = 0,  StartDate = new DateTime(2026, 4, 1),  ServiceCoordinator = "D. Kwan" },
            new Participant { FullName = "Nina Ruiz",     Initials = "NR", ProgramId = mjc.Id,      Status = ParticipantStatus.Active,      AttendancePct = 88, StartDate = new DateTime(2024, 1, 1),  ServiceCoordinator = "D. Kwan" },
            new Participant { FullName = "Leo Walsh",     Initials = "LW", ProgramId = mjc.Id,      Status = ParticipantStatus.Active,      AttendancePct = 75, StartDate = new DateTime(2023, 3, 1),  ServiceCoordinator = "D. Kwan" },
        };

        var pathwaysPts = new[]
        {
            new Participant { FullName = "Aiden Cole",  Initials = "AC", ProgramId = pathways.Id, Status = ParticipantStatus.Active,    AttendancePct = 92, StartDate = new DateTime(2023, 9, 1), ServiceCoordinator = "R. Alvarez" },
            new Participant { FullName = "Mia Soto",    Initials = "MS", ProgramId = pathways.Id, Status = ParticipantStatus.Active,    AttendancePct = 88, StartDate = new DateTime(2022, 9, 1), ServiceCoordinator = "R. Alvarez" },
            new Participant { FullName = "Jasmine V.",  Initials = "JV", ProgramId = pathways.Id, Status = ParticipantStatus.Active,    AttendancePct = 95, StartDate = new DateTime(2023, 1, 1), ServiceCoordinator = "R. Alvarez" },
            new Participant { FullName = "Tyler B.",    Initials = "TB", ProgramId = pathways.Id, Status = ParticipantStatus.Attention, AttendancePct = 71, StartDate = new DateTime(2024, 1, 1), ServiceCoordinator = "R. Alvarez" },
            new Participant { FullName = "Emma G.",     Initials = "EG", ProgramId = pathways.Id, Status = ParticipantStatus.Active,    AttendancePct = 91, StartDate = new DateTime(2022, 9, 1), ServiceCoordinator = "R. Alvarez" },
        };

        var mantecaPts = new[]
        {
            new Participant { FullName = "Sam Obi",    Initials = "SO", ProgramId = manteca.Id, Status = ParticipantStatus.Active, AttendancePct = 84, StartDate = new DateTime(2023, 9, 1), ServiceCoordinator = "T. Cho" },
            new Participant { FullName = "Priya K.",   Initials = "PK", ProgramId = manteca.Id, Status = ParticipantStatus.Active, AttendancePct = 84, StartDate = new DateTime(2022, 10, 1), ServiceCoordinator = "T. Cho" },
            new Participant { FullName = "Jordan F.",  Initials = "JF", ProgramId = manteca.Id, Status = ParticipantStatus.Active, AttendancePct = 79, StartDate = new DateTime(2023, 6, 1), ServiceCoordinator = "T. Cho" },
            new Participant { FullName = "Lucia M.",   Initials = "LM", ProgramId = manteca.Id, Status = ParticipantStatus.Active, AttendancePct = 82, StartDate = new DateTime(2023, 1, 1), ServiceCoordinator = "T. Cho" },
        };

        db.Participants.AddRange(mjcPts);
        db.Participants.AddRange(pathwaysPts);
        db.Participants.AddRange(mantecaPts);

        // ── Sessions ──────────────────────────────────────────────────────────

        db.Sessions.AddRange(
            new Session { ProgramId = mjc.Id,      Date = new DateTime(2026, 6, 9),  Room = "FH 152", TimeRange = "9:00–11:30 AM",       Label = "Session 15" },
            new Session { ProgramId = mjc.Id,      Date = new DateTime(2026, 6, 11), Room = "FH 152", TimeRange = "9:00–11:30 AM",       Label = "Session 16" },
            new Session { ProgramId = mjc.Id,      Date = new DateTime(2026, 6, 13), Room = "FH 152", TimeRange = "9:00–11:30 AM",       Label = "Session 17" },
            new Session { ProgramId = pathways.Id, Date = new DateTime(2026, 6, 10), Room = "Room C", TimeRange = "10:00 AM–12:30 PM",   Label = "Session 12" },
            new Session { ProgramId = pathways.Id, Date = new DateTime(2026, 6, 12), Room = "Room C", TimeRange = "10:00 AM–12:30 PM",   Label = "Session 13" },
            new Session { ProgramId = pathways.Id, Date = new DateTime(2026, 6, 17), Room = "Room C", TimeRange = "10:00 AM–12:30 PM",   Label = "Session 14" },
            new Session { ProgramId = manteca.Id,  Date = new DateTime(2026, 6, 9),  Room = "Gym B",  TimeRange = "1:00–3:00 PM",        Label = "Session 8"  },
            new Session { ProgramId = manteca.Id,  Date = new DateTime(2026, 6, 12), Room = "Gym B",  TimeRange = "1:00–3:00 PM",        Label = "Session 9"  },
            new Session { ProgramId = manteca.Id,  Date = new DateTime(2026, 6, 16), Room = "Gym B",  TimeRange = "1:00–3:00 PM",        Label = "Session 10" }
        );

        // ── Calendar events ───────────────────────────────────────────────────

        db.CalendarEvents.AddRange(
            new CalendarEvent { Title = "Spring Showcase Rehearsal", Date = new DateTime(2026, 6, 14), ProgramId = mjc.Id,      Meta = "Full cast · Main Stage",            IsUpcoming = true },
            new CalendarEvent { Title = "End-of-term Assessment",    Date = new DateTime(2026, 6, 20), ProgramId = mjc.Id,      Meta = "Room B · Rachel M.",                 IsUpcoming = true },
            new CalendarEvent { Title = "Regional Theatre Workshop",  Date = new DateTime(2026, 6, 21), ProgramId = pathways.Id, Meta = "All Pathways students · Bus 9 AM",  IsUpcoming = true },
            new CalendarEvent { Title = "Mid-term Check-in",          Date = new DateTime(2026, 6, 24), ProgramId = pathways.Id, Meta = "Room C · Joss K.",                  IsUpcoming = true },
            new CalendarEvent { Title = "Script Reading",             Date = new DateTime(2026, 6, 18), ProgramId = manteca.Id,  Meta = "Room A · Devon P.",                 IsUpcoming = true },
            new CalendarEvent { Title = "Parent Observation",         Date = new DateTime(2026, 6, 25), ProgramId = manteca.Id,  Meta = "Gym B · All families welcome",      IsUpcoming = true }
        );

        // ── Staff-program assignments ─────────────────────────────────────────

        db.StaffProgramAssignments.AddRange(
            new StaffProgramAssignment { StaffMemberId = rachel.Id, ProgramId = mjc.Id      },
            new StaffProgramAssignment { StaffMemberId = devon.Id,  ProgramId = mjc.Id      },
            new StaffProgramAssignment { StaffMemberId = nina.Id,   ProgramId = mjc.Id      },
            new StaffProgramAssignment { StaffMemberId = joss.Id,   ProgramId = pathways.Id },
            new StaffProgramAssignment { StaffMemberId = maya.Id,   ProgramId = pathways.Id },
            new StaffProgramAssignment { StaffMemberId = devon.Id,  ProgramId = manteca.Id  },
            new StaffProgramAssignment { StaffMemberId = alex.Id,   ProgramId = manteca.Id  }
        );

        // ── Onboarding items (Joss K. in progress at 65%) ────────────────────

        db.OnboardingItems.AddRange(
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Documents",      Label = "Background check",        IsCompleted = true,  CompletedDate = new DateTime(2024, 8, 20) },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Documents",      Label = "TB test results",         IsCompleted = true,  CompletedDate = new DateTime(2024, 8, 20) },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Documents",      Label = "I-9 / ID verification",   IsCompleted = true,  CompletedDate = new DateTime(2024, 8, 20) },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Documents",      Label = "Signed offer letter",     IsCompleted = true,  CompletedDate = new DateTime(2024, 8, 15) },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Systems Setup",  Label = "Email account created",   IsCompleted = true,  CompletedDate = new DateTime(2024, 8, 16) },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Systems Setup",  Label = "Google Workspace access", IsCompleted = true,  CompletedDate = new DateTime(2024, 8, 16) },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Systems Setup",  Label = "CRM access granted",      IsCompleted = false },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Training",       Label = "Org orientation complete",IsCompleted = false },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Training",       Label = "Program handbook review", IsCompleted = false },
            new OnboardingItem { StaffMemberId = joss.Id, Section = "Training",       Label = "First session shadowing", IsCompleted = false }
        );

        // ── Projects & Tasks ──────────────────────────────────────────────────

        var springShow = new Project { Title = "Spring Show 2026", Type = ProjectType.Production, Status = "inprogress", Scope = "Full musical production" };
        var onboarding = new Project { Title = "New Staff Onboarding", Type = ProjectType.Staff, Status = "inprogress", Scope = "Q3 2026 hires" };
        var compliance = new Project { Title = "Compliance Renewals", Type = ProjectType.Admin, Status = "planning", DueDate = new DateTime(2026, 7, 1) };
        db.Projects.AddRange(springShow, onboarding, compliance);

        await db.SaveChangesAsync();

        db.Tasks.AddRange(
            new ProjectTask { ProjectId = springShow.Id, Name = "Finalize script selection",   Status = Domain.Enums.TaskStatus.Done,       Priority = TaskPriority.High,   AssignedToId = rachel.Id },
            new ProjectTask { ProjectId = springShow.Id, Name = "Cast all roles",               Status = Domain.Enums.TaskStatus.InProgress, Priority = TaskPriority.High,   AssignedToId = devon.Id,  DueDate = new DateTime(2026, 6, 20) },
            new ProjectTask { ProjectId = springShow.Id, Name = "Reserve venue",                Status = Domain.Enums.TaskStatus.InProgress, Priority = TaskPriority.Medium, AssignedToId = rachel.Id, DueDate = new DateTime(2026, 6, 15) },
            new ProjectTask { ProjectId = springShow.Id, Name = "Print programs",               Status = Domain.Enums.TaskStatus.Upcoming,   Priority = TaskPriority.Low,    DueDate = new DateTime(2026, 7, 1) },
            new ProjectTask { ProjectId = onboarding.Id, Name = "Complete CRM access for Joss", Status = Domain.Enums.TaskStatus.Upcoming,   Priority = TaskPriority.High,   AssignedToId = rachel.Id, DueDate = new DateTime(2026, 6, 16) },
            new ProjectTask { ProjectId = onboarding.Id, Name = "Schedule orientation",         Status = Domain.Enums.TaskStatus.InProgress, Priority = TaskPriority.Medium, AssignedToId = joss.Id },
            new ProjectTask { ProjectId = compliance.Id, Name = "Audit POS expiry dates",       Status = Domain.Enums.TaskStatus.Upcoming,   Priority = TaskPriority.High,   AssignedToId = rachel.Id, DueDate = new DateTime(2026, 6, 18) },
            new ProjectTask { ProjectId = compliance.Id, Name = "Request renewal for Marcus T.", Status = Domain.Enums.TaskStatus.Upcoming,   Priority = TaskPriority.High,   AssignedToId = rachel.Id, DueDate = new DateTime(2026, 6, 13), IsOverdue = true }
        );

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds sample attendance for TODAY so the attendance page and dashboard show realistic
    /// activity: a session per program with most participants marked Present (every 4th Absent)
    /// and a couple of notes. Idempotent — skips entirely once any attendance note exists.
    /// </summary>
    public static async Task SeedSampleAttendanceAsync(AppDbContext db)
    {
        if (await db.AttendanceNotes.AnyAsync()) return;

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var programs = await db.Programs.ToListAsync();

        foreach (var program in programs)
        {
            var active = await db.Participants
                .Where(p => p.ProgramId == program.Id && p.Status == ParticipantStatus.Active)
                .ToListAsync();
            if (active.Count == 0) continue;

            // Get-or-create today's session for this program.
            var session = await db.Sessions
                .FirstOrDefaultAsync(s => s.ProgramId == program.Id && s.Date >= today && s.Date < tomorrow);
            if (session is null)
            {
                session = new Session
                {
                    ProgramId = program.Id,
                    Date = today,
                    Room = program.DefaultLocation,
                    TimeRange = program.StartTime is { } st && program.EndTime is { } et
                        ? $"{st:h:mm tt}–{et:h:mm tt}"
                        : null,
                    Status = SessionStatus.Open,
                };
                db.Sessions.Add(session);
                await db.SaveChangesAsync();
            }

            // Ensure a record per active participant.
            var existing = (await db.AttendanceRecords.Where(r => r.SessionId == session.Id).ToListAsync())
                .ToDictionary(r => r.ParticipantId);
            var records = new List<AttendanceRecord>();
            foreach (var p in active)
            {
                if (!existing.TryGetValue(p.Id, out var rec))
                {
                    rec = new AttendanceRecord { ParticipantId = p.Id, SessionId = session.Id, Status = AttendanceStatus.Unmarked };
                    db.AttendanceRecords.Add(rec);
                }
                records.Add(rec);
            }
            await db.SaveChangesAsync();

            // Mark a realistic spread: every 4th Absent, the rest Present.
            for (var i = 0; i < records.Count; i++)
                records[i].Status = (i % 4 == 3) ? AttendanceStatus.Absent : AttendanceStatus.Present;

            // A couple of sample notes on the first participants.
            if (records.Count > 0)
                db.AttendanceNotes.Add(new AttendanceNote
                {
                    AttendanceRecordId = records[0].Id,
                    Content = $"Great energy in {program.Name} warm-ups today.",
                    NoteType = "observation",
                });
            if (records.Count > 1)
                db.AttendanceNotes.Add(new AttendanceNote
                {
                    AttendanceRecordId = records[1].Id,
                    Content = "Arrived late — follow up with family about transportation.",
                    NoteType = "concern",
                });

            await db.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seeds the demo Script Library so the Scripts page has content in every environment
    /// (explicit client request — the Scripts page is kept as a working UI demo, #18).
    /// Idempotent — skips entirely once any script exists. Links each script to the programs
    /// it names, when those programs exist (matched by slug); the "productions" tag has no
    /// backing program and is simply not linked.
    /// </summary>
    public static async Task SeedScriptsAsync(AppDbContext db)
    {
        if (await db.Scripts.AnyAsync()) return;

        var programIdBySlug = await db.Programs.ToDictionaryAsync(p => p.Slug, p => p.Id);

        // (Title, Subtitle, Type, Status, IsOriginal, IsAdapted, CastMin, CastMax, Duration, ProgramSlugs)
        var rows = new (string Title, string? Subtitle, ScriptType Type, ScriptStatus Status,
            bool IsOriginal, bool IsAdapted, int? CastMin, int? CastMax, string? Duration, string[] Slugs)[]
        {
            ("The Magic Garden", "An original musical in two acts", ScriptType.Musical, ScriptStatus.Active, true, false, 12, 18, "55 min", new[] { "pathways" }),
            ("Cinderella", "Adapted for performing arts", ScriptType.Play, ScriptStatus.Active, false, true, 8, 12, "40 min", new[] { "mjc" }),
            ("Under the Sea", "A musical celebration", ScriptType.Musical, ScriptStatus.Active, true, false, 6, 10, "35 min", new[] { "manteca" }),
            ("The Brave Little Star", "Scene collection — one act per program group", ScriptType.Scene, ScriptStatus.Active, true, false, 4, 8, "20 min / scene", new[] { "mjc", "pathways", "manteca" }),
            ("Stardust", "New original musical — in development", ScriptType.Musical, ScriptStatus.Draft, true, false, null, null, "TBD", Array.Empty<string>()),
            ("A Midsummer Dream", "Adapted from Shakespeare", ScriptType.Play, ScriptStatus.Archived, false, true, 15, 20, "60 min", Array.Empty<string>()),
            ("Rainbow Road", "A musical journey", ScriptType.Musical, ScriptStatus.Archived, true, false, 10, 14, "45 min", new[] { "mjc", "pathways" }),
            ("The Lighthouse Keeper", null, ScriptType.Play, ScriptStatus.Archived, true, false, 6, 8, "30 min", new[] { "manteca" }),
        };

        foreach (var row in rows)
        {
            var script = new Script
            {
                Title = row.Title,
                Subtitle = row.Subtitle,
                Type = row.Type,
                Status = row.Status,
                IsOriginal = row.IsOriginal,
                IsAdapted = row.IsAdapted,
                CastMin = row.CastMin,
                CastMax = row.CastMax,
                Duration = row.Duration,
            };
            db.Scripts.Add(script);

            foreach (var slug in row.Slugs)
            {
                if (programIdBySlug.TryGetValue(slug, out var programId))
                    db.ScriptPrograms.Add(new ScriptProgram { Script = script, ProgramId = programId });
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds the Games Library (~57 games from the Annual Programming Calendar), tagging each
    /// to the shared taxonomy by slug. Idempotent — skips entirely once any game exists. The
    /// taxonomy (ObjectiveAreas + SubSkills) is seeded by the AddSkillsTaxonomy migration, so it
    /// is present by the time this runs; if it somehow isn't, this no-ops rather than crashing.
    /// </summary>
    public static async Task SeedGamesLibraryAsync(AppDbContext db)
    {
        if (await db.Games.AnyAsync()) return;

        var areaBySlug = await db.ObjectiveAreas.ToDictionaryAsync(a => a.Slug, a => a.Id);
        var subBySlug = await db.SubSkills.ToDictionaryAsync(s => s.Slug, s => s.Id);
        if (areaBySlug.Count == 0 || subBySlug.Count == 0) return; // taxonomy not seeded yet

        foreach (var row in GameSeedData.Games)
        {
            if (!areaBySlug.TryGetValue(row.AreaSlug, out var areaId)) continue;

            var game = new Game
            {
                Name = row.Name,
                Source = row.Source,
                Category = row.Category,
                CategoryLabel = row.CategoryLabel,
                Tiers = row.Tiers,
                PrimaryObjectiveAreaId = areaId,
                Description = row.Description,
                BestForVariations = row.BestFor,
                WhenToUse = row.WhenToUse,
            };
            db.Games.Add(game);

            var order = 0;
            foreach (var (slug, isPrimary) in row.SubGoals)
            {
                if (!subBySlug.TryGetValue(slug, out var subId)) continue;
                db.GameSubGoals.Add(new GameSubGoal
                {
                    GameId = game.Id,
                    SubSkillId = subId,
                    IsPrimary = isPrimary,
                    SortOrder = order++,
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
