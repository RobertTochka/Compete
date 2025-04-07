using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAward_Lobbies_LobbyId",
                table: "UserAward");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAward_Users_UserId",
                table: "UserAward");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAward",
                table: "UserAward");

            migrationBuilder.RenameTable(
                name: "UserAward",
                newName: "Awards");

            migrationBuilder.RenameIndex(
                name: "IX_UserAward_UserId",
                table: "Awards",
                newName: "IX_Awards_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAward_LobbyId",
                table: "Awards",
                newName: "IX_Awards_LobbyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Awards",
                table: "Awards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Lobbies_LobbyId",
                table: "Awards",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Users_UserId",
                table: "Awards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Lobbies_LobbyId",
                table: "Awards");

            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Users_UserId",
                table: "Awards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Awards",
                table: "Awards");

            migrationBuilder.RenameTable(
                name: "Awards",
                newName: "UserAward");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_UserId",
                table: "UserAward",
                newName: "IX_UserAward_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_LobbyId",
                table: "UserAward",
                newName: "IX_UserAward_LobbyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAward",
                table: "UserAward",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAward_Lobbies_LobbyId",
                table: "UserAward",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAward_Users_UserId",
                table: "UserAward",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
