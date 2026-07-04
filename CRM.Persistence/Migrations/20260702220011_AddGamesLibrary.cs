using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGamesLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    CategoryLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tiers = table.Column<int>(type: "int", nullable: false),
                    PrimaryObjectiveAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BestForVariations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WhenToUse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_ObjectiveAreas_PrimaryObjectiveAreaId",
                        column: x => x.PrimaryObjectiveAreaId,
                        principalTable: "ObjectiveAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameSubGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubSkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSubGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSubGoals_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSubGoals_SubSkills_SubSkillId",
                        column: x => x.SubSkillId,
                        principalTable: "SubSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Name",
                table: "Games",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PrimaryObjectiveAreaId",
                table: "Games",
                column: "PrimaryObjectiveAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSubGoals_GameId_SubSkillId",
                table: "GameSubGoals",
                columns: new[] { "GameId", "SubSkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSubGoals_SubSkillId",
                table: "GameSubGoals",
                column: "SubSkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSubGoals");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
