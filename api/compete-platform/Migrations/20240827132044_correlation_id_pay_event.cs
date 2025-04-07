using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace compete_poco.Migrations
{
    public partial class correlation_id_pay_event : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "PayEvents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "PayEvents");
        }
    }
}
