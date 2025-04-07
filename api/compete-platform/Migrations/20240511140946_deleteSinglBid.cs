using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class deleteSinglBid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SingleBid",
                table: "Lobbies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SingleBid",
                table: "Lobbies",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
