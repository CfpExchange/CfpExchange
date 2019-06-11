using Microsoft.EntityFrameworkCore.Migrations;

namespace CfpExchange.Migrations
{
    public partial class AddTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventTags",
                table: "Cfps",
				nullable: true, defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTags",
                table: "Cfps");
        }
    }
}
