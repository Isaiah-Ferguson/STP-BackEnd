using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteGroupRoster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StarGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RosterAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StarGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quarter = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CountedInRatio = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RosterAssignments_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterAssignments_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RosterAssignments_Staff_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RosterAssignments_StarGroups_StarGroupId",
                        column: x => x.StarGroupId,
                        principalTable: "StarGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RosterAssignments_AssignedStaffId",
                table: "RosterAssignments",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterAssignments_ParticipantId_Year_Quarter",
                table: "RosterAssignments",
                columns: new[] { "ParticipantId", "Year", "Quarter" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RosterAssignments_SiteId",
                table: "RosterAssignments",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterAssignments_StarGroupId",
                table: "RosterAssignments",
                column: "StarGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Slug",
                table: "Sites",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StarGroups_Slug",
                table: "StarGroups",
                column: "Slug",
                unique: true);

            // ── Seed the reference lookups: 3 sites + 3 star groups ──
            // Fixed IDs + timestamp so every environment shares identical rows and roster
            // assignments reference stable identifiers. RosterAssignment rows themselves are
            // real operational data, entered by management — not seeded.
            var seed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "Sites",
                columns: new[] { "Id", "Name", "Slug", "SortOrder", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-000000000001"), "MJC Modesto", "mjc-modesto", 1, seed, seed },
                    { new Guid("33333333-3333-3333-3333-000000000002"), "Manteca",     "manteca",     2, seed, seed },
                    { new Guid("33333333-3333-3333-3333-000000000003"), "Pathways",    "pathways",    3, seed, seed },
                });

            migrationBuilder.InsertData(
                table: "StarGroups",
                columns: new[] { "Id", "Name", "Slug", "SortOrder", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-000000000001"), "Bright Stars",    "bright-stars",    1, seed, seed },
                    { new Guid("44444444-4444-4444-4444-000000000002"), "Glowing Stars",   "glowing-stars",   2, seed, seed },
                    { new Guid("44444444-4444-4444-4444-000000000003"), "Sparkling Stars", "sparkling-stars", 3, seed, seed },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RosterAssignments");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "StarGroups");
        }
    }
}
