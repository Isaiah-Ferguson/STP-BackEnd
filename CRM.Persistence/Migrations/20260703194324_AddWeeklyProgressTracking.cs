using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyProgressTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonthlyProgressSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubSkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonthKey = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    SuggestedLevel = table.Column<int>(type: "int", nullable: false),
                    SummedScore = table.Column<int>(type: "int", nullable: false),
                    ScoredWeekCount = table.Column<int>(type: "int", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedByStaffMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyProgressSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyProgressSnapshots_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonthlyProgressSnapshots_Staff_ConfirmedByStaffMemberId",
                        column: x => x.ConfirmedByStaffMemberId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MonthlyProgressSnapshots_SubSkills_SubSkillId",
                        column: x => x.SubSkillId,
                        principalTable: "SubSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScoreThresholds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    MinAverage = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreThresholds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyDataEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubSkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MonthKey = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    WeekDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    RecordedByStaffMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyDataEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyDataEntries_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyDataEntries_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WeeklyDataEntries_Staff_RecordedByStaffMemberId",
                        column: x => x.RecordedByStaffMemberId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WeeklyDataEntries_SubSkills_SubSkillId",
                        column: x => x.SubSkillId,
                        principalTable: "SubSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyFocusSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonthKey = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    SubSkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyFocusSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyFocusSkills_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyFocusSkills_SubSkills_SubSkillId",
                        column: x => x.SubSkillId,
                        principalTable: "SubSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyProgressSnapshots_ConfirmedByStaffMemberId",
                table: "MonthlyProgressSnapshots",
                column: "ConfirmedByStaffMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyProgressSnapshots_MonthKey_SubSkillId",
                table: "MonthlyProgressSnapshots",
                columns: new[] { "MonthKey", "SubSkillId" });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyProgressSnapshots_ParticipantId_SubSkillId_MonthKey",
                table: "MonthlyProgressSnapshots",
                columns: new[] { "ParticipantId", "SubSkillId", "MonthKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyProgressSnapshots_SubSkillId",
                table: "MonthlyProgressSnapshots",
                column: "SubSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreThresholds_Level",
                table: "ScoreThresholds",
                column: "Level",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyDataEntries_ParticipantId_MonthKey",
                table: "WeeklyDataEntries",
                columns: new[] { "ParticipantId", "MonthKey" });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyDataEntries_ParticipantId_SubSkillId_MonthKey_WeekNumber",
                table: "WeeklyDataEntries",
                columns: new[] { "ParticipantId", "SubSkillId", "MonthKey", "WeekNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyDataEntries_RecordedByStaffMemberId",
                table: "WeeklyDataEntries",
                column: "RecordedByStaffMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyDataEntries_SessionId",
                table: "WeeklyDataEntries",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyDataEntries_SubSkillId",
                table: "WeeklyDataEntries",
                column: "SubSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyFocusSkills_ProgramId_MonthKey_WeekNumber_SubSkillId",
                table: "WeeklyFocusSkills",
                columns: new[] { "ProgramId", "MonthKey", "WeekNumber", "SubSkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyFocusSkills_SubSkillId",
                table: "WeeklyFocusSkills",
                column: "SubSkillId");

            // ── Seed the month-end derivation thresholds (average of scored weeks, 0–3) ──
            // Novice 0.0 (floor) · Intermediate ≥1.5 · Expert ≥2.5. Editable later without code.
            // ProgressLevel enum stored as int: Novice=0, Intermediate=1, Expert=2, NotApplicable=3.
            var seed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "ScoreThresholds",
                columns: new[] { "Id", "Level", "MinAverage", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-000000000001"), 0, 0.0, seed, seed },
                    { new Guid("55555555-5555-5555-5555-000000000002"), 1, 1.5, seed, seed },
                    { new Guid("55555555-5555-5555-5555-000000000003"), 2, 2.5, seed, seed },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyProgressSnapshots");

            migrationBuilder.DropTable(
                name: "ScoreThresholds");

            migrationBuilder.DropTable(
                name: "WeeklyDataEntries");

            migrationBuilder.DropTable(
                name: "WeeklyFocusSkills");
        }
    }
}
