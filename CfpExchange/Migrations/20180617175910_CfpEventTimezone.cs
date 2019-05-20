using Microsoft.EntityFrameworkCore.Migrations;

namespace CfpExchange.Migrations
{
    public partial class CfpEventTimezone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventTimezone",
                table: "Cfps",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTimezone",
                table: "Cfps");
        }
    }
}
