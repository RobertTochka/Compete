using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class addNewDomainDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Config_FreezeTime",
                table: "Lobbies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchFormat",
                table: "Lobbies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayersAmount",
                table: "Lobbies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Config_FreezeTime",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "MatchFormat",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "PlayersAmount",
                table: "Lobbies");
        }
    }
}
