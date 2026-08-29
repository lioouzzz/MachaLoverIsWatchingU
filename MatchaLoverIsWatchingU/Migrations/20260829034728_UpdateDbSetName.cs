using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchaLoverIsWatchingU.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbSetName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "Lessons",
                newName: "WatchedProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WatchedProducts",
                table: "WatchedProducts",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WatchedProducts",
                table: "WatchedProducts");

            migrationBuilder.RenameTable(
                name: "WatchedProducts",
                newName: "Lessons");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "Id");
        }
    }
}
