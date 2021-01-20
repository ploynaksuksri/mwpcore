using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mwp.Migrations
{
    public partial class Split_Tenant_Extra_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantResources_SaasTenants_TenantId",
                schema: "mwp",
                table: "TenantResources");

            migrationBuilder.DropIndex(
                name: "IX_TenantResources_TenantId",
                schema: "mwp",
                table: "TenantResources");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SaasTenants");

            migrationBuilder.DropColumn(
                name: "TenantParentId",
                table: "SaasTenants");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "mwp",
                table: "TenantResources",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantExId",
                schema: "mwp",
                table: "TenantResources",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TenantExes",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: true, defaultValue: true),
                    TenantParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantExes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantExes_SaasTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "SaasTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantResources_TenantExId",
                schema: "mwp",
                table: "TenantResources",
                column: "TenantExId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantExes_TenantId",
                schema: "mwp",
                table: "TenantExes",
                column: "TenantId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantResources_TenantExes_TenantExId",
                schema: "mwp",
                table: "TenantResources",
                column: "TenantExId",
                principalSchema: "mwp",
                principalTable: "TenantExes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantResources_TenantExes_TenantExId",
                schema: "mwp",
                table: "TenantResources");

            migrationBuilder.DropTable(
                name: "TenantExes",
                schema: "mwp");

            migrationBuilder.DropIndex(
                name: "IX_TenantResources_TenantExId",
                schema: "mwp",
                table: "TenantResources");

            migrationBuilder.DropColumn(
                name: "TenantExId",
                schema: "mwp",
                table: "TenantResources");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "mwp",
                table: "TenantResources",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SaasTenants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantParentId",
                table: "SaasTenants",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TenantResources_TenantId",
                schema: "mwp",
                table: "TenantResources",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantResources_SaasTenants_TenantId",
                schema: "mwp",
                table: "TenantResources",
                column: "TenantId",
                principalTable: "SaasTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
