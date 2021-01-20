using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mwp.Migrations
{
    public partial class Add_Form_Parent_Detail_And_CurrentVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentVersion",
                schema: "mwp",
                table: "Forms",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ParentDetail",
                schema: "mwp",
                table: "Forms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentVersion",
                schema: "mwp",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "ParentDetail",
                schema: "mwp",
                table: "Forms");
        }
    }
}
