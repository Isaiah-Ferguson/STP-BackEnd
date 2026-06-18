using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BackfillProgramSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill structured schedules for the seeded programs. The seeder only runs on
            // an empty database, so existing databases need this to populate the new columns.
            // MeetingDays is the [Flags] sum: Mon=2, Tue=4, Wed=8, Thu=16, Fri=32.
            migrationBuilder.Sql(
                "UPDATE [Programs] SET [MeetingDays] = 42, [StartTime] = '09:00:00', [EndTime] = '11:30:00' WHERE [Slug] = 'mjc';");      // Mon|Wed|Fri
            migrationBuilder.Sql(
                "UPDATE [Programs] SET [MeetingDays] = 20, [StartTime] = '10:00:00', [EndTime] = '12:30:00' WHERE [Slug] = 'pathways';"); // Tue|Thu
            migrationBuilder.Sql(
                "UPDATE [Programs] SET [MeetingDays] = 18, [StartTime] = '13:00:00', [EndTime] = '15:00:00' WHERE [Slug] = 'manteca';");  // Mon|Thu
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [Programs] SET [MeetingDays] = 0, [StartTime] = NULL, [EndTime] = NULL WHERE [Slug] IN ('mjc', 'pathways', 'manteca');");
        }
    }
}
