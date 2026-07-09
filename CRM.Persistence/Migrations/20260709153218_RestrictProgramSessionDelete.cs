using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestrictProgramSessionDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Programs_ProgramId",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Programs_ProgramId",
                table: "Sessions",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Programs_ProgramId",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Programs_ProgramId",
                table: "Sessions",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
