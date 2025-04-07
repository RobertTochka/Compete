using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class uniqueConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PayEvents_PayState_PaymentId",
                table: "PayEvents");

            migrationBuilder.CreateIndex(
                name: "IX_PayEvents_PayState_PaymentId",
                table: "PayEvents",
                columns: new[] { "PayState", "PaymentId" },
                unique: true,
                filter: "\"PaymentId\" IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PayEvents_PayState_PaymentId",
                table: "PayEvents");

            migrationBuilder.CreateIndex(
                name: "IX_PayEvents_PayState_PaymentId",
                table: "PayEvents",
                columns: new[] { "PayState", "PaymentId" },
                filter: "\"PaymentId\" IS NOT NULL");
        }
    }
}
