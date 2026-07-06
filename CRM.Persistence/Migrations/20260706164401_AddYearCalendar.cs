using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddYearCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarThemes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    ThemeTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ThemeSubtitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    KeyArtsDatesText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FeaturedGamesText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AlternativeOptionsText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProductionPhase = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ProgrammingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LegendArc = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarThemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyArtsDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    DateText = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observance = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ObservanceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProgrammingTieIn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyArtsDates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarThemes_Month",
                table: "CalendarThemes",
                column: "Month",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KeyArtsDates_Month_SortOrder",
                table: "KeyArtsDates",
                columns: new[] { "Month", "SortOrder" });

            // Seed the Annual Programming Calendar: 12 monthly themes + recurring key arts dates.
            var seed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
migrationBuilder.InsertData(
    table: "CalendarThemes",
    columns: new[] { "Id", "Month", "ThemeTitle", "ThemeSubtitle", "KeyArtsDatesText", "FeaturedGamesText", "AlternativeOptionsText", "ProductionPhase", "ProgrammingNotes", "LegendArc", "CreatedAt", "UpdatedAt" },
    values: new object[,]
    {
        { new Guid("77777777-7777-7777-7777-000000000001"), 1, "Fresh Starts", "New Beginnings & Foundations", "New Year's · MLK Jr. Day · Lunar New Year", "FOUNDATIONAL RESET + THEATER OLYMPICS: Actor Suit, Show Me Your Moves!, Zip Zap Zop, Pass the Pulse, Popsico, Twizzle, 123 Focus, Compliment/Appreciate/Pass", "Jingle Bells, Group Counting, Fuzzy Wuzzy, Emotion Check-in, Goal-setting circle", "POST-NUTCRACKER RECOVERY — Foundational return", "First two weeks: foundations only. Week 3: light scene work. MLK Day for reflection.", 0, seed, seed },
        { new Guid("77777777-7777-7777-7777-000000000002"), 2, "Storytelling", "Voices & Stories: Black History Month", "Black History Month · Valentine's Day", "Story Circle, Five Minute Fairytale, Animalz, Spotlight: Black artists, Gibberish Interpreter, Photograph", "Dinner Party (Black artists), Tableau storytelling", "Pre-spring skill building", "Pair every artist with an age-appropriate clip.", null, seed, seed },
        { new Guid("77777777-7777-7777-7777-000000000003"), 3, "Voices & Vision", "Women in the Arts: Women's History Month", "Women's History Month · World Theatre Day (3/27)", "Character Interview, Park Bench, 1-2-3-4 Open Scene, Audition prep", "Twin Interview, The Normal Guy, Restaurant", "SPRING SHOW REHEARSALS BEGIN", "World Theatre Day (3/27) — mini-showcase anchor.", 1, seed, seed },
        { new Guid("77777777-7777-7777-7777-000000000004"), 4, "Poetry & Movement", "Language, Rhythm, Spring Show Prep", "National Poetry Month · Earth Day · Shakespeare's Birthday · Dance Day", "Spring Show rehearsals, Tongue twisters, Vocal Scales, Poetry monologues, Choreography clean-up", "Body percussion, Silent Scene, Earth devising", "SPRING SHOW: rehearsal heavy", "Tech week typically end of April. Two full run-throughs.", 1, seed, seed },
        { new Guid("77777777-7777-7777-7777-000000000005"), 5, "Showtime", "Spring Show + Celebration", "AAPI Heritage · Jewish American Heritage · Mother's Day · Memorial Day", "PERFORMANCE WEEKS, Warm-up rituals, Performance + strike, POST-SHOW: Compliment/Appreciate/Pass", "Open mic share, 'What I learned' tableau, AAPI artists", "SPRING SHOW PERFORMS", "Cast party within 1 week. No new heavy material until June.", 1, seed, seed },
        { new Guid("77777777-7777-7777-7777-000000000006"), 6, "Reset & Welcome", "Pride, Identity & Foundational Reset", "Pride Month · Juneteenth · World Music Day", "THEATER OLYMPICS FORMAT: Actor Suit, Show Me Your Moves, Zip Zap Zop, Pass the Pulse, Popsico, Look Up and ACT!", "'This is me' tableau, Pride devising, Juneteenth: Black joy", "FOUNDATIONAL RESET — New Stars onboarding", "Pair returning Stars with new Stars as buddies.", 0, seed, seed },
        { new Guid("77777777-7777-7777-7777-000000000007"), 7, "Bold Choices", "Independence & Big Choices", "Disability Pride Month · Independence Day", "Director Says, Slow Motion Samurai, Whoosh Ball, Yes Let's!, Machines, Vocal projection", "Meter Tag, Machine variations, Clap Focus, Photograph", "Summer intensive — No production pressure", "Outdoor sessions encouraged.", null, seed, seed },
        { new Guid("77777777-7777-7777-7777-000000000008"), 8, "Ensemble", "Group Mind & Collaboration", "Women's Equality Day · Tell a Joke Day", "Yes And, Quick Change, Dinner Party, Normal Guy, Freeze, Machines, Restaurant", "1-2-3-4 open scenes, Gibberish, Joke writing", "Summer concludes — Prep for fall", "Low-pressure showcase end of month.", null, seed, seed },
        { new Guid("77777777-7777-7777-7777-000000000009"), 9, "Heritage & Return", "Hispanic Heritage Month begins (9/15)", "Hispanic Heritage Month · Labor Day · Peace Day", "Rhythm games, Salsa basics, Bilingual Popsico, Spotlight: Latine artists, Audition prep, Character Interview", "Heritage storytelling, Family origin tableaus", "Fall kickoff — Nutcracker auditions late", "Confirm Nutcracker cast by 9/30.", 2, seed, seed },
        { new Guid("77777777-7777-7777-7777-00000000000a"), 10, "Character & Transformation", "Masks, Characters, Bold Transformations", "Arts & Humanities Month · LGBTQ History Month · Halloween", "Animalz advanced, Park Bench, Normal Guy, Photograph, Director's Coming, Nutcracker rehearsals begin", "Halloween devising, queer artists spotlight, Quick Change, Mask workshop", "NUTCRACKER REHEARSALS BEGIN", "Costume measurements end of month.", 2, seed, seed },
        { new Guid("77777777-7777-7777-7777-00000000000b"), 11, "Gratitude & Ensemble", "Native American Heritage Month · Deep Rehearsal", "Native American Heritage Month · Veterans Day · Thanksgiving", "Nutcracker rehearsals, Hands trust game, Group Counting, Machines, Breath work", "Indigenous storytelling, Maria Tallchief, Gratitude reflection", "NUTCRACKER: HEAVY REHEARSAL — Off-book deadlines", "Thanksgiving: short focused rehearsals.", 2, seed, seed },
        { new Guid("77777777-7777-7777-7777-00000000000c"), 12, "The Big Show", "Nutcracker + Holiday Traditions", "Nutcracker performances · Hanukkah · Solstice · Christmas · Kwanzaa", "PERFORMANCE WEEKS: Tech + dress, Actor Suit warm-ups, Vocal warm-ups, Performances + strike", "Mini-showcase, Global winter traditions, Breathe Like the Ocean", "NUTCRACKER PERFORMS", "Cast/family celebration weekend after closing.", 2, seed, seed },
    });

migrationBuilder.InsertData(
    table: "KeyArtsDates",
    columns: new[] { "Id", "Month", "SortOrder", "DateText", "Observance", "ObservanceType", "ProgrammingTieIn", "CreatedAt", "UpdatedAt" },
    values: new object[,]
    {
        { new Guid("88888888-8888-8888-8888-000000000001"), 1, 1, "1/1", "New Year's Day", "Holiday", "Goal-setting circle", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000002"), 1, 2, "3rd Mon", "MLK Jr. Day", "Federal", "Reflection on artists & justice", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000003"), 1, 3, "Varies", "Lunar New Year", "Cultural", "Asian theater & dance traditions", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000004"), 2, 1, "Full month", "Black History Month", "Heritage", "Center Black artists", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000005"), 2, 2, "2/14", "Valentine's Day", "Holiday", "Love-themed monologues", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000006"), 2, 3, "3rd Mon", "Presidents' Day", "Federal", "Historical figure character study", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000007"), 3, 1, "Full month", "Women's History Month", "Heritage", "Spotlight women in performing arts", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000008"), 3, 2, "3/8", "Int'l Women's Day", "Observance", "Featured women-artist day", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000009"), 3, 3, "3/27", "World Theatre Day", "Arts", "MINI-SHOWCASE anchor", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000000a"), 4, 1, "Full month", "National Poetry Month", "Arts", "Daily poetry warm-ups", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000000b"), 4, 2, "4/22", "Earth Day", "Observance", "Earth-themed devising", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000000c"), 4, 3, "4/23", "Shakespeare's Birthday", "Arts", "Shakespeare insult game", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000000d"), 4, 4, "4/29", "Int'l Dance Day", "Arts", "Flash mob or dance share-out", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000000e"), 5, 1, "Full month", "AAPI Heritage Month", "Heritage", "Spotlight AAPI artists", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000000f"), 5, 2, "Full month", "Jewish American Heritage", "Heritage", "Spotlight Jewish artists", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000010"), 5, 3, "2nd Sun", "Mother's Day", "Holiday", "Family-themed share", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000011"), 5, 4, "Last Mon", "Memorial Day", "Federal", "No programming", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000012"), 6, 1, "Full month", "Pride Month", "Heritage", "Identity, queer artists", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000013"), 6, 2, "6/19", "Juneteenth", "Federal", "Black joy in performance", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000014"), 6, 3, "6/21", "World Music Day", "Arts", "Global music exploration", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000015"), 6, 4, "3rd Sun", "Father's Day", "Holiday", "Family-themed share", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000016"), 7, 1, "Full month", "Disability Pride Month", "Heritage", "Ali Stroker, Marlee Matlin", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000017"), 7, 2, "7/4", "Independence Day", "Federal", "No programming", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000018"), 7, 3, "7/30", "Int'l Friendship Day", "Observance", "Ensemble bonding", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000019"), 8, 1, "8/16", "Tell a Joke Day", "Fun", "Joke writing/performing", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000001a"), 9, 1, "1st Mon", "Labor Day", "Federal", "No programming", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000001b"), 9, 2, "9/15-10/15", "Hispanic Heritage", "Heritage", "Spotlight Latine artists", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000001c"), 9, 3, "9/21", "Int'l Day of Peace", "Observance", "Image theater on peace", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000001d"), 9, 4, "Varies", "Rosh Hashanah", "Religious", "Adjust schedule", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000001e"), 10, 1, "Full month", "Arts & Humanities Month", "Arts", "Major anchor", seed, seed },
        { new Guid("88888888-8888-8888-8888-00000000001f"), 10, 2, "Full month", "LGBTQ History Month", "Heritage", "Spotlight queer artists", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000020"), 10, 3, "2nd Mon", "Indigenous Peoples' Day", "Observance", "Maria Tallchief", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000021"), 10, 4, "10/31", "Halloween", "Holiday", "Character intensive", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000022"), 11, 1, "Full month", "Native American Heritage", "Heritage", "Spotlight Native artists", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000023"), 11, 2, "11/11", "Veterans Day", "Federal", "Veterans-themed work", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000024"), 11, 3, "4th Thu", "Thanksgiving", "Federal", "Gratitude circle", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000025"), 11, 4, "Varies", "Diwali", "Cultural", "South Asian traditions", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000026"), 12, 1, "Varies", "Hanukkah", "Religious", "Festival of lights", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000027"), 12, 2, "12/21", "Winter Solstice", "Seasonal", "Reflection on the year", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000028"), 12, 3, "12/25", "Christmas", "Federal", "Studio closed", seed, seed },
        { new Guid("88888888-8888-8888-8888-000000000029"), 12, 4, "12/26-1/1", "Kwanzaa", "Cultural", "Seven principles", seed, seed },
    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarThemes");

            migrationBuilder.DropTable(
                name: "KeyArtsDates");
        }
    }
}
