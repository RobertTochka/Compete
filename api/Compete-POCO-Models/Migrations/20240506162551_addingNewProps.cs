using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class addingNewProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Headshots",
                table: "UserStat",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "Matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamWinner",
                table: "Lobbies",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TeamId",
                table: "Matches",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Team_TeamId",
                table: "Matches",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Team_TeamId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_TeamId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Headshots",
                table: "UserStat");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "TeamWinner",
                table: "Lobbies");
        }
    }
}
