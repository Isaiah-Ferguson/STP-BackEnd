using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGameBacklogAndPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgeModifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GroupAgeLevel = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Modification = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TeacherSuggested = table.Column<bool>(type: "bit", nullable: false),
                    TeacherSuggestedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeModifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgeModifications_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AgeModifications_Staff_TeacherSuggestedId",
                        column: x => x.TeacherSuggestedId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GameIdeas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StatusNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SourceInspiration = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TargetCategory = table.Column<int>(type: "int", nullable: true),
                    TeacherSuggested = table.Column<bool>(type: "bit", nullable: false),
                    TeacherSuggestedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromotedGameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameIdeas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameIdeas_Staff_TeacherSuggestedId",
                        column: x => x.TeacherSuggestedId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PerStarPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MonthKey = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    PrimaryTier = table.Column<int>(type: "int", nullable: false),
                    PriorityObjectiveAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrioritySubSkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MonthlyGoal = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HowIllSupport = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerStarPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerStarPlans_ObjectiveAreas_PriorityObjectiveAreaId",
                        column: x => x.PriorityObjectiveAreaId,
                        principalTable: "ObjectiveAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PerStarPlans_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerStarPlans_Staff_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PerStarPlans_SubSkills_PrioritySubSkillId",
                        column: x => x.PrioritySubSkillId,
                        principalTable: "SubSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgeModifications_GameId",
                table: "AgeModifications",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_AgeModifications_TeacherSuggestedId",
                table: "AgeModifications",
                column: "TeacherSuggestedId");

            migrationBuilder.CreateIndex(
                name: "IX_GameIdeas_TeacherSuggestedId",
                table: "GameIdeas",
                column: "TeacherSuggestedId");

            migrationBuilder.CreateIndex(
                name: "IX_PerStarPlans_AssignedStaffId",
                table: "PerStarPlans",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_PerStarPlans_ParticipantId_MonthKey",
                table: "PerStarPlans",
                columns: new[] { "ParticipantId", "MonthKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerStarPlans_PriorityObjectiveAreaId",
                table: "PerStarPlans",
                column: "PriorityObjectiveAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_PerStarPlans_PrioritySubSkillId",
                table: "PerStarPlans",
                column: "PrioritySubSkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgeModifications");

            migrationBuilder.DropTable(
                name: "GameIdeas");

            migrationBuilder.DropTable(
                name: "PerStarPlans");
        }
    }
}
