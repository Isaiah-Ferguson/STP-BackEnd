using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalBankAndNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoalBankEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    SectionNumber = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HasGrowingEdge = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SubSkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalBankEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonthKey = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    PrimaryLevel = table.Column<int>(type: "int", nullable: false),
                    ProgressNarrative = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    GoalsCarryOver = table.Column<bool>(type: "bit", nullable: false),
                    NextMonthUpdate = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlySummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlySummaries_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyNoteSelections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonthKey = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    GoalBankEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyNoteSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyNoteSelections_GoalBankEntries_GoalBankEntryId",
                        column: x => x.GoalBankEntryId,
                        principalTable: "GoalBankEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeeklyNoteSelections_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoalBankEntries_Kind_SectionNumber_Level",
                table: "GoalBankEntries",
                columns: new[] { "Kind", "SectionNumber", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlySummaries_ParticipantId_MonthKey",
                table: "MonthlySummaries",
                columns: new[] { "ParticipantId", "MonthKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyNoteSelections_GoalBankEntryId",
                table: "WeeklyNoteSelections",
                column: "GoalBankEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyNoteSelections_ParticipantId_MonthKey_WeekNumber_Kind",
                table: "WeeklyNoteSelections",
                columns: new[] { "ParticipantId", "MonthKey", "WeekNumber", "Kind" },
                unique: true);

            // Seed the Section-6 example bank (54 entries: 18 section·level tags × Strength/Area/NewGoal).
            var seed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "GoalBankEntries",
                columns: new[] { "Id", "Kind", "SectionNumber", "Level", "Text", "HasGrowingEdge", "IsActive", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-000000000001"), 0, 1, 0, "Settled into the opening circle and named or showed how they were feeling in their own way.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000002"), 1, 1, 0, "Working on settling into the opening circle — sometimes still chooses to pass on the check-in.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000003"), 2, 1, 0, "Join the opening check-in this week — try sharing a feeling in a new way, or pushing past the usual pass.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000004"), 0, 1, 1, "Accepted a castmate's idea in an ensemble scene and built on it instead of redirecting.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000005"), 1, 1, 1, "Building comfort accepting a castmate's idea without changing the direction of the scene.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000006"), 2, 1, 1, "Stay with a castmate's offer through a full ensemble scene this week — let their idea lead all the way through.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000007"), 0, 1, 2, "Read the room during ensemble work and adjusted their choice so a peer could have their moment.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000008"), 1, 1, 2, "Working on noticing when to step back so a peer can take the spotlight in a scene.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000009"), 2, 1, 2, "Adapt to one unexpected change in a scene this week — adjust to a partner's new choice without dropping the moment.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000000a"), 0, 2, 0, "Projected a line or used a clear gesture or expression on cue so the audience could follow.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000000b"), 1, 2, 0, "Building volume and clarity so a spoken line or gesture carries to the back of the room.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000000c"), 2, 2, 0, "Deliver one line or cue with a little more projection or a clearer gesture this week.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000000d"), 0, 2, 1, "Followed along in the script and read their line (or used their adaptive cue) at the right moment.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000000e"), 1, 2, 1, "Working on tracking the script and coming in on their line with less prompting.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000000f"), 2, 2, 1, "Track the script and come in on their own line once this week without a cue from staff.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000010"), 0, 2, 2, "Delivered their memorized lines (or choreography cues) this week without prompting.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000011"), 1, 2, 2, "Memorizing lines or cues is in progress — repetition is helping it stick.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000012"), 2, 2, 2, "Add one more line or cue to memory beyond last week's count.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000013"), 0, 2, 2, "Made an independent choice about their character, line delivery, or part in the number.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000014"), 1, 2, 2, "Growing in making their own creative choices rather than waiting to be told.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000015"), 2, 2, 2, "Make one creative choice of their own this week — a character beat, a movement, or how to say a line.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000016"), 0, 3, 0, "Followed the rehearsal flow and started a warm-up or activity when it was time.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000017"), 1, 3, 0, "Working on moving into each rehearsal activity with less prompting.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000018"), 2, 3, 0, "Start one rehearsal activity on their own this week when the schedule calls for it.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000019"), 0, 3, 1, "Moved between rehearsal activities (warm-up to scene to notes) without losing focus.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000001a"), 1, 3, 1, "Building smoother transitions between activities — settling in a little faster each time.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000001b"), 2, 3, 1, "Move into the next activity this week without dropping focus — even when the rehearsal shifts gears.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000001c"), 0, 3, 2, "Sustained attention through a full run-through and saw their part through to the end.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000001d"), 1, 3, 2, "Building stamina to stay focused through a complete run or rehearsal piece.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000001e"), 2, 3, 2, "Hold focus through one full run-through this week from top to finish.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000001f"), 0, 4, 0, "Greeted a guest artist, visitor, or audience member in their own way.  (community)", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000020"), 1, 4, 0, "Working on greeting or acknowledging a guest or audience member with a little more ease.  (community)", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000021"), 2, 4, 0, "Greet one new face this week — a guest artist, visitor, or family member at pickup.  (community)", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000022"), 0, 4, 1, "Took part in a community-facing moment — an open rehearsal, showcase, or outing.  (community)", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000023"), 1, 4, 1, "Building comfort participating in community-facing performances and events.  (community)", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000024"), 2, 4, 1, "Take part in one community-facing moment this week — an open rehearsal or showcase number.  (community)", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000025"), 0, 4, 2, "Represented Shining Stars at an outside event and supported a peer while there.  (community)", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000026"), 1, 4, 2, "Growing more comfortable collaborating with peers at events beyond our usual class.  (community)", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000027"), 2, 4, 2, "At the next outside event, partner with or introduce themselves to one new person.  (community)", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000028"), 0, 5, 0, "Followed the warm-up or choreography movement and stayed with the group's shape.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000029"), 1, 5, 0, "Working on following choreography and holding the movement through a full count.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000002a"), 2, 5, 0, "Hold one movement or choreography sequence a little longer or with more presence this week.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000002b"), 0, 5, 1, "Brought a character to life in a scene with their own expression and energy.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000002c"), 1, 5, 1, "Building a character in a scene — finding the voice, face, and body of the role.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000002d"), 2, 5, 1, "Try a new character choice this week — step a little outside their usual range.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000002e"), 0, 5, 1, "Held their choreography through a full number alongside the ensemble.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-00000000002f"), 1, 5, 1, "Building stamina to hold choreography through a complete number.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000030"), 2, 5, 1, "Stay with the choreography through one full number this week without dropping out.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000031"), 0, 5, 2, "Performed their part in a solo or group number with confidence and on cue.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000032"), 1, 5, 2, "Building confidence and breath to carry their part in a musical number.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000033"), 2, 5, 2, "Add one more phrase, harmony, or dynamic to their number beyond last week.", true, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000034"), 0, 5, 2, "Sustained their character through a full scene or piece, even through transitions.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000035"), 1, 5, 2, "Building stamina to stay in character through a complete scene or piece.", false, true, seed, seed },
                    { new Guid("66666666-6666-6666-6666-000000000036"), 2, 5, 2, "Hold character through one full piece this week — stay with it even when the scene shifts.", true, true, seed, seed },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlySummaries");

            migrationBuilder.DropTable(
                name: "WeeklyNoteSelections");

            migrationBuilder.DropTable(
                name: "GoalBankEntries");
        }
    }
}
