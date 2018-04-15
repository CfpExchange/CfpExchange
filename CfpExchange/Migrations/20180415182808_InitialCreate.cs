using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CfpExchange.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cfps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CfpAdded = table.Column<DateTime>(nullable: false),
                    CfpEndDate = table.Column<DateTime>(nullable: false),
                    CfpStartDate = table.Column<DateTime>(nullable: false),
                    CfpUrl = table.Column<string>(nullable: true),
                    ClicksToCfpUrl = table.Column<int>(nullable: false),
                    EventDescription = table.Column<string>(nullable: true),
                    EventEndDate = table.Column<DateTime>(nullable: false),
                    EventImage = table.Column<string>(nullable: true),
                    EventLocationLat = table.Column<double>(nullable: false),
                    EventLocationLng = table.Column<double>(nullable: false),
                    EventLocationName = table.Column<string>(nullable: true),
                    EventName = table.Column<string>(nullable: true),
                    EventStartDate = table.Column<DateTime>(nullable: false),
                    EventUrl = table.Column<string>(nullable: true),
                    SubmittedByName = table.Column<string>(nullable: true),
                    Views = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cfps", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cfps");
        }
    }
}
