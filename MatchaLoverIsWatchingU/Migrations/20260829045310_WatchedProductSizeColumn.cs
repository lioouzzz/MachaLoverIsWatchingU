using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchaLoverIsWatchingU.Migrations
{
    /// <inheritdoc />
    public partial class WatchedProductSizeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "WatchedProducts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "WatchedProducts");
        }
    }
}
