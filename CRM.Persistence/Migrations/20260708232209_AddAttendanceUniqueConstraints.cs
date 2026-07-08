using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_ProgramId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_SessionId",
                table: "AttendanceRecords");

            // ── De-duplicate existing data before the unique indexes are applied ──────────
            // The old check-then-insert code (with GroupBy(...).First() papering over it)
            // could create duplicate sessions per (ProgramId, Date) and duplicate records per
            // (SessionId, ParticipantId). Creating a UNIQUE index on top of duplicates fails,
            // so collapse them first. This is idempotent and a no-op on a clean database.

            // 1. Merge duplicate sessions: repoint every record from a duplicate session onto
            //    the canonical (earliest) session for that (ProgramId, Date).
            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT Id,
                           FIRST_VALUE(Id) OVER (PARTITION BY ProgramId, [Date] ORDER BY CreatedAt, Id) AS CanonicalId
                    FROM Sessions
                )
                UPDATE ar
                SET ar.SessionId = r.CanonicalId
                FROM AttendanceRecords ar
                JOIN ranked r ON ar.SessionId = r.Id
                WHERE r.Id <> r.CanonicalId;");

            // 2. Delete the now-orphaned duplicate sessions.
            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (PARTITION BY ProgramId, [Date] ORDER BY CreatedAt, Id) AS rn
                    FROM Sessions
                )
                DELETE FROM Sessions WHERE Id IN (SELECT Id FROM ranked WHERE rn > 1);");

            // 3. Move notes off duplicate records onto the record we will keep for each
            //    (SessionId, ParticipantId). Prefer a marked record (Status <> 2 = Unmarked)
            //    over an unmarked one, then the earliest. Notes cascade-delete with their
            //    record, so they must be repointed before the duplicates are removed.
            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT Id,
                           FIRST_VALUE(Id) OVER (
                               PARTITION BY SessionId, ParticipantId
                               ORDER BY CASE WHEN Status = 2 THEN 1 ELSE 0 END, CreatedAt, Id) AS KeepId
                    FROM AttendanceRecords
                )
                UPDATE n
                SET n.AttendanceRecordId = r.KeepId
                FROM AttendanceNotes n
                JOIN ranked r ON n.AttendanceRecordId = r.Id
                WHERE r.Id <> r.KeepId;");

            // 4. Delete the duplicate attendance records.
            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (
                               PARTITION BY SessionId, ParticipantId
                               ORDER BY CASE WHEN Status = 2 THEN 1 ELSE 0 END, CreatedAt, Id) AS rn
                    FROM AttendanceRecords
                )
                DELETE FROM AttendanceRecords WHERE Id IN (SELECT Id FROM ranked WHERE rn > 1);");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ProgramId_Date",
                table: "Sessions",
                columns: new[] { "ProgramId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_SessionId_ParticipantId",
                table: "AttendanceRecords",
                columns: new[] { "SessionId", "ParticipantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_ProgramId_Date",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_SessionId_ParticipantId",
                table: "AttendanceRecords");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ProgramId",
                table: "Sessions",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_SessionId",
                table: "AttendanceRecords",
                column: "SessionId");
        }
    }
}
