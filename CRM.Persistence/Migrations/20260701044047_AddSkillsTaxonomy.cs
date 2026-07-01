using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillsTaxonomy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectiveAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorHex = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectiveAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObjectiveAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SectionNumber = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubSkills_ObjectiveAreas_ObjectiveAreaId",
                        column: x => x.ObjectiveAreaId,
                        principalTable: "ObjectiveAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveAreas_Slug",
                table: "ObjectiveAreas",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubSkills_ObjectiveAreaId_SortOrder",
                table: "SubSkills",
                columns: new[] { "ObjectiveAreaId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SubSkills_Slug",
                table: "SubSkills",
                column: "Slug",
                unique: true);

            // ── Seed the shared skill taxonomy: 6 objective areas + 18 sub-skills ──
            // Canonical reference data with fixed IDs and timestamp so every environment
            // (and every later phase that references these rows — Games, weekly tracker,
            // cohort roll-up) sees identical, stable identifiers. This migration is the
            // sole seeder of the taxonomy; no DataSeeder path duplicates it.
            var seed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "ObjectiveAreas",
                columns: new[] { "Id", "Name", "Slug", "ColorHex", "SortOrder", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-000000000001"), "Social Skill Development",     "social",                "#c9a227", 1, seed, seed },
                    { new Guid("11111111-1111-1111-1111-000000000002"), "Communication Goals",          "communication",         "#378add", 2, seed, seed },
                    { new Guid("11111111-1111-1111-1111-000000000003"), "Executive Functioning Skills", "executive-functioning", "#2e9e5b", 3, seed, seed },
                    { new Guid("11111111-1111-1111-1111-000000000004"), "Community Integration Goals",  "community-integration", "#e07b39", 4, seed, seed },
                    { new Guid("11111111-1111-1111-1111-000000000005"), "Performing Arts Development",  "performing-arts",       "#8a5fc4", 5, seed, seed },
                    { new Guid("11111111-1111-1111-1111-000000000006"), "Multi-Area",                   "multi-area",            "#6b6960", 6, seed, seed },
                });

            migrationBuilder.InsertData(
                table: "SubSkills",
                columns: new[] { "Id", "ObjectiveAreaId", "Name", "Slug", "SectionNumber", "SortOrder", "IsActive", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    // Section 1 — Social Skill Development
                    { new Guid("22222222-2222-2222-2222-000000000001"), new Guid("11111111-1111-1111-1111-000000000001"), "Group Collaboration",              "group-collaboration",              1, 1, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-000000000002"), new Guid("11111111-1111-1111-1111-000000000001"), "Emotional Regulation & Empathy",   "emotional-regulation-empathy",     1, 2, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-000000000003"), new Guid("11111111-1111-1111-1111-000000000001"), "Perspective Taking & Adaptability","perspective-taking-adaptability",  1, 3, true, seed, seed },
                    // Section 2 — Communication Goals
                    { new Guid("22222222-2222-2222-2222-000000000004"), new Guid("11111111-1111-1111-1111-000000000002"), "Verbal Communication",             "verbal-communication",             2, 1, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-000000000005"), new Guid("11111111-1111-1111-1111-000000000002"), "Non-verbal Communication",         "non-verbal-communication",         2, 2, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-000000000006"), new Guid("11111111-1111-1111-1111-000000000002"), "Script Reading / Memorization",    "script-reading-memorization",      2, 3, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-000000000007"), new Guid("11111111-1111-1111-1111-000000000002"), "Self Direction & Choice Making",   "self-direction-choice-making",     2, 4, true, seed, seed },
                    // Section 3 — Executive Functioning Skills
                    { new Guid("22222222-2222-2222-2222-000000000008"), new Guid("11111111-1111-1111-1111-000000000003"), "Following a Schedule",             "following-a-schedule",             3, 1, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-000000000009"), new Guid("11111111-1111-1111-1111-000000000003"), "Task Initiation",                  "task-initiation",                  3, 2, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-00000000000a"), new Guid("11111111-1111-1111-1111-000000000003"), "Completing Tasks",                 "completing-tasks",                 3, 3, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-00000000000b"), new Guid("11111111-1111-1111-1111-000000000003"), "Transitions",                      "transitions",                      3, 4, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-00000000000c"), new Guid("11111111-1111-1111-1111-000000000003"), "Attention and Focus",              "attention-and-focus",              3, 5, true, seed, seed },
                    // Section 4 — Community Integration Goals
                    { new Guid("22222222-2222-2222-2222-00000000000d"), new Guid("11111111-1111-1111-1111-000000000004"), "Public Interaction",               "public-interaction",               4, 1, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-00000000000e"), new Guid("11111111-1111-1111-1111-000000000004"), "Community Participation",          "community-participation",          4, 2, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-00000000000f"), new Guid("11111111-1111-1111-1111-000000000004"), "Peer Collaboration Outside Class", "peer-collaboration-outside-class", 4, 3, true, seed, seed },
                    // Section 5 — Performing Arts Development
                    { new Guid("22222222-2222-2222-2222-000000000010"), new Guid("11111111-1111-1111-1111-000000000005"), "Acting",                           "acting",                           5, 1, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-000000000011"), new Guid("11111111-1111-1111-1111-000000000005"), "Dance & Movement",                 "dance-movement",                   5, 2, true, seed, seed },
                    { new Guid("22222222-2222-2222-2222-000000000012"), new Guid("11111111-1111-1111-1111-000000000005"), "Musical & Vocal Training",         "musical-vocal-training",           5, 3, true, seed, seed },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubSkills");

            migrationBuilder.DropTable(
                name: "ObjectiveAreas");
        }
    }
}
