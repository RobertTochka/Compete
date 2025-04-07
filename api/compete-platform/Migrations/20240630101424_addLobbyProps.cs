using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class addLobbyProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EdgeConnectTimeOnFirstMap",
                table: "Lobbies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WaitToStartTime",
                table: "Lobbies",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_Users_SteamId",
                table: "Users",
                column: "SteamId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_SteamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EdgeConnectTimeOnFirstMap",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "WaitToStartTime",
                table: "Lobbies");
        }
    }
}
