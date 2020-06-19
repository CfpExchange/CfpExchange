using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CfpExchange.Migrations
{
    public partial class TravelAndAccommodation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvidedExpenses",
                table: "Cfps");

            migrationBuilder.AddColumn<DateTime>(
                name: "CfpDecisionDate",
                table: "Cfps",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ProvidesAccommodation",
                table: "Cfps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProvidesTravelAssistance",
                table: "Cfps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Cfps",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CfpDecisionDate",
                table: "Cfps");

            migrationBuilder.DropColumn(
                name: "ProvidesAccommodation",
                table: "Cfps");

            migrationBuilder.DropColumn(
                name: "ProvidesTravelAssistance",
                table: "Cfps");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Cfps");

            migrationBuilder.AddColumn<int>(
                name: "ProvidedExpenses",
                table: "Cfps",
                nullable: false,
                defaultValue: 0);
        }
    }
}
