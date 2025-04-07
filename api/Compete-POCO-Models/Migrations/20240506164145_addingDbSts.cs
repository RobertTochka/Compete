using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class addingDbSts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBid_Lobbies_LobbyId",
                table: "UserBid");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBid_Users_UserId",
                table: "UserBid");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStat_Matches_MatchId",
                table: "UserStat");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStat_Users_UserId",
                table: "UserStat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserStat",
                table: "UserStat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBid",
                table: "UserBid");

            migrationBuilder.RenameTable(
                name: "UserStat",
                newName: "UserStats");

            migrationBuilder.RenameTable(
                name: "UserBid",
                newName: "Bids");

            migrationBuilder.RenameIndex(
                name: "IX_UserStat_UserId",
                table: "UserStats",
                newName: "IX_UserStats_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserStat_MatchId",
                table: "UserStats",
                newName: "IX_UserStats_MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBid_UserId",
                table: "Bids",
                newName: "IX_Bids_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBid_LobbyId",
                table: "Bids",
                newName: "IX_Bids_LobbyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserStats",
                table: "UserStats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bids",
                table: "Bids",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Lobbies_LobbyId",
                table: "Bids",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Users_UserId",
                table: "Bids",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStats_Matches_MatchId",
                table: "UserStats",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStats_Users_UserId",
                table: "UserStats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Lobbies_LobbyId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Users_UserId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStats_Matches_MatchId",
                table: "UserStats");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStats_Users_UserId",
                table: "UserStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserStats",
                table: "UserStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bids",
                table: "Bids");

            migrationBuilder.RenameTable(
                name: "UserStats",
                newName: "UserStat");

            migrationBuilder.RenameTable(
                name: "Bids",
                newName: "UserBid");

            migrationBuilder.RenameIndex(
                name: "IX_UserStats_UserId",
                table: "UserStat",
                newName: "IX_UserStat_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserStats_MatchId",
                table: "UserStat",
                newName: "IX_UserStat_MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_UserId",
                table: "UserBid",
                newName: "IX_UserBid_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_LobbyId",
                table: "UserBid",
                newName: "IX_UserBid_LobbyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserStat",
                table: "UserStat",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBid",
                table: "UserBid",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBid_Lobbies_LobbyId",
                table: "UserBid",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBid_Users_UserId",
                table: "UserBid",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStat_Matches_MatchId",
                table: "UserStat",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStat_Users_UserId",
                table: "UserStat",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
