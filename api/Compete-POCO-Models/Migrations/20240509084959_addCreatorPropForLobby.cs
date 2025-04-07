using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class addCreatorPropForLobby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatorId",
                table: "Lobbies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_CreatorId",
                table: "Lobbies",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lobbies_Users_CreatorId",
                table: "Lobbies",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lobbies_Users_CreatorId",
                table: "Lobbies");

            migrationBuilder.DropIndex(
                name: "IX_Lobbies_CreatorId",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Lobbies");
        }
    }
}
