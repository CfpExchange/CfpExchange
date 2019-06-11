using Microsoft.EntityFrameworkCore.Migrations;

namespace CfpExchange.Migrations
{
    public partial class CfpEventTwitterHandle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventTwitterHandle",
                table: "Cfps",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTwitterHandle",
                table: "Cfps");
        }
    }
}
