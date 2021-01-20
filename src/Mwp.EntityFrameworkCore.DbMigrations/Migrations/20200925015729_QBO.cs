using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mwp.Migrations
{
    public partial class QBO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QboTenants",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    MwpTenantId = table.Column<Guid>(nullable: true),
                    QboTenantId = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QboTenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QboTokens",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    AccessToken = table.Column<string>(maxLength: 4096, nullable: false),
                    IdToken = table.Column<string>(maxLength: 4096, nullable: true),
                    RefreshToken = table.Column<string>(maxLength: 512, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(nullable: false),
                    IsRevoked = table.Column<bool>(nullable: false),
                    IsRefreshed = table.Column<bool>(nullable: false),
                    MwpUserId = table.Column<Guid>(nullable: false),
                    RefreshTokenExpiresAtUtc = table.Column<DateTime>(nullable: false),
                    QboTenantId = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QboTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_QBOTENANTS_MWP_TENANT_ID",
                schema: "mwp",
                table: "QboTenants",
                column: "MwpTenantId");

            migrationBuilder.CreateIndex(
                name: "IDX_QBOTOKENS_MWP_USER_ID",
                schema: "mwp",
                table: "QboTokens",
                column: "MwpUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QboTenants",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "QboTokens",
                schema: "mwp");
        }
    }
}