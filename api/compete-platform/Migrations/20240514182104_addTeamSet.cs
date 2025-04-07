using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class addTeamSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Team_TeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Chats_ChatId",
                table: "Team");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Lobbies_LobbyId",
                table: "Team");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamUser_Team_TeamsId",
                table: "TeamUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Team",
                table: "Team");

            migrationBuilder.RenameTable(
                name: "Team",
                newName: "Teams");

            migrationBuilder.RenameIndex(
                name: "IX_Team_LobbyId",
                table: "Teams",
                newName: "IX_Teams_LobbyId");

            migrationBuilder.RenameIndex(
                name: "IX_Team_ChatId",
                table: "Teams",
                newName: "IX_Teams_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_TeamId",
                table: "Matches",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Chats_ChatId",
                table: "Teams",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Lobbies_LobbyId",
                table: "Teams",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUser_Teams_TeamsId",
                table: "TeamUser",
                column: "TeamsId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_TeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Chats_ChatId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Lobbies_LobbyId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamUser_Teams_TeamsId",
                table: "TeamUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teams",
                table: "Teams");

            migrationBuilder.RenameTable(
                name: "Teams",
                newName: "Team");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_LobbyId",
                table: "Team",
                newName: "IX_Team_LobbyId");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_ChatId",
                table: "Team",
                newName: "IX_Team_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Team",
                table: "Team",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Team_TeamId",
                table: "Matches",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Chats_ChatId",
                table: "Team",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Lobbies_LobbyId",
                table: "Team",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUser_Team_TeamsId",
                table: "TeamUser",
                column: "TeamsId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
