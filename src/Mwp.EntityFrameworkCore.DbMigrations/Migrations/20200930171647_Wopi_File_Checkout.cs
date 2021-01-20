using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mwp.Migrations
{
    public partial class Wopi_File_Checkout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CheckoutBy",
                schema: "mwp",
                table: "WopiFiles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckoutTimestamp",
                schema: "mwp",
                table: "WopiFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckoutBy",
                schema: "mwp",
                table: "WopiFiles");

            migrationBuilder.DropColumn(
                name: "CheckoutTimestamp",
                schema: "mwp",
                table: "WopiFiles");
        }
    }
}
