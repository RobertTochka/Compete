using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class addExplicitRelationBetweenLobbyAndServer2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lobbies_Servers_ServerId1",
                table: "Lobbies");

            migrationBuilder.DropIndex(
                name: "IX_Lobbies_ServerId1",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "ServerId1",
                table: "Lobbies");

            migrationBuilder.AlterColumn<int>(
                name: "ServerId",
                table: "Lobbies",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_ServerId",
                table: "Lobbies",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lobbies_Servers_ServerId",
                table: "Lobbies",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lobbies_Servers_ServerId",
                table: "Lobbies");

            migrationBuilder.DropIndex(
                name: "IX_Lobbies_ServerId",
                table: "Lobbies");

            migrationBuilder.AlterColumn<long>(
                name: "ServerId",
                table: "Lobbies",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ServerId1",
                table: "Lobbies",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_ServerId1",
                table: "Lobbies",
                column: "ServerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Lobbies_Servers_ServerId1",
                table: "Lobbies",
                column: "ServerId1",
                principalTable: "Servers",
                principalColumn: "Id");
        }
    }
}
