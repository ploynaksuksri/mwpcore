using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mwp.Migrations
{
    public partial class Add_SoftDelete_Form_And_Submission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "mwp",
                table: "Submissions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "mwp",
                table: "Submissions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "mwp",
                table: "Submissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "mwp",
                table: "Forms",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "mwp",
                table: "Forms",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "mwp",
                table: "Forms",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "mwp",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "mwp",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "mwp",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "mwp",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "mwp",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "mwp",
                table: "Forms");
        }
    }
}
