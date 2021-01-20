using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mwp.Migrations
{
    public partial class Mwp_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mwp");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SaasTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantParentId",
                table: "SaasTenants",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CloudServiceLocations",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudServiceLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CloudServiceProviders",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformName = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudServiceProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CloudServiceTypes",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTypeName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Data = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    HierarchicalPath = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFormConfigs",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    FormBuilderConfig = table.Column<string>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFormConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WopiFiles",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true),
                    SubmissionId = table.Column<Guid>(nullable: true),
                    LockValue = table.Column<string>(maxLength: 1024, nullable: true),
                    LockExpires = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WopiFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XeroTenants",
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
                    XeroTenantId = table.Column<Guid>(nullable: false),
                    TenantType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    MwpTenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XeroTenants", x => x.Id);
                    table.UniqueConstraint("AK_XeroTenants_XeroTenantId", x => x.XeroTenantId);
                });

            migrationBuilder.CreateTable(
                name: "XeroTokens",
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
                    AccessToken = table.Column<string>(maxLength: 2048, nullable: true),
                    IdToken = table.Column<string>(maxLength: 2048, nullable: true),
                    RefreshToken = table.Column<string>(maxLength: 512, nullable: true),
                    ExpiresAtUtc = table.Column<DateTime>(nullable: false),
                    IsRevoked = table.Column<bool>(nullable: false),
                    IsRefreshed = table.Column<bool>(nullable: false),
                    MwpUserId = table.Column<Guid>(nullable: false),
                    AuthenticationEventId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XeroTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CloudServices",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudServiceProviderId = table.Column<int>(nullable: false),
                    ServiceName = table.Column<string>(maxLength: 100, nullable: false),
                    CloudServiceTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudServices_CloudServiceProviders_CloudServiceProviderId",
                        column: x => x.CloudServiceProviderId,
                        principalSchema: "mwp",
                        principalTable: "CloudServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CloudServices_CloudServiceTypes_CloudServiceTypeId",
                        column: x => x.CloudServiceTypeId,
                        principalSchema: "mwp",
                        principalTable: "CloudServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    FormId = table.Column<Guid>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "mwp",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WopiFileHistories",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    WopiFileId = table.Column<Guid>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    Revision = table.Column<int>(nullable: false),
                    FileIdInStorage = table.Column<Guid>(nullable: false),
                    BaseFileName = table.Column<string>(maxLength: 250, nullable: false),
                    Size = table.Column<int>(nullable: false),
                    LastModificationDetail = table.Column<string>(nullable: true),
                    LastModifiedUsers = table.Column<string>(maxLength: 1024, nullable: true),
                    LastModifiedSessionId = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WopiFileHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WopiFileHistories_WopiFiles_WopiFileId",
                        column: x => x.WopiFileId,
                        principalSchema: "mwp",
                        principalTable: "WopiFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XeroConnections",
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
                    XeroConnectionId = table.Column<Guid>(nullable: false),
                    XeroTenantId = table.Column<Guid>(nullable: false),
                    MwpUserId = table.Column<Guid>(nullable: true),
                    AuthenticationEventId = table.Column<Guid>(nullable: false),
                    IsConnected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XeroConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XeroConnections_XeroTenants_XeroTenantId",
                        column: x => x.XeroTenantId,
                        principalSchema: "mwp",
                        principalTable: "XeroTenants",
                        principalColumn: "XeroTenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CloudServiceOptions",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloudServiceId = table.Column<int>(nullable: false),
                    OptionName = table.Column<string>(maxLength: 100, nullable: false),
                    OptionDesc = table.Column<string>(maxLength: 200, nullable: false),
                    IsShared = table.Column<bool>(nullable: false),
                    IsProvisionRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudServiceOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudServiceOptions_CloudServices_CloudServiceId",
                        column: x => x.CloudServiceId,
                        principalSchema: "mwp",
                        principalTable: "CloudServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SharedResources",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CloudServiceLocationId = table.Column<int>(nullable: false),
                    CloudServiceOptionId = table.Column<int>(nullable: false),
                    IsTrial = table.Column<bool>(nullable: false),
                    SecretName = table.Column<string>(maxLength: 127, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedResources_CloudServiceLocations_CloudServiceLocationId",
                        column: x => x.CloudServiceLocationId,
                        principalSchema: "mwp",
                        principalTable: "CloudServiceLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedResources_CloudServiceOptions_CloudServiceOptionId",
                        column: x => x.CloudServiceOptionId,
                        principalSchema: "mwp",
                        principalTable: "CloudServiceOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantResources",
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
                    CloudServiceOptionId = table.Column<int>(nullable: false),
                    CloudServiceLocationId = table.Column<int>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    SubscriptionId = table.Column<string>(maxLength: 50, nullable: true),
                    ResourceGroup = table.Column<string>(maxLength: 90, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ProvisionStatus = table.Column<int>(nullable: false),
                    ConnectionString = table.Column<string>(maxLength: 1024, nullable: true),
                    ServerName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantResources_CloudServiceLocations_CloudServiceLocationId",
                        column: x => x.CloudServiceLocationId,
                        principalSchema: "mwp",
                        principalTable: "CloudServiceLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantResources_CloudServiceOptions_CloudServiceOptionId",
                        column: x => x.CloudServiceOptionId,
                        principalSchema: "mwp",
                        principalTable: "CloudServiceOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantResources_SaasTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "SaasTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CloudServiceOptions_CloudServiceId",
                schema: "mwp",
                table: "CloudServiceOptions",
                column: "CloudServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudServices_CloudServiceProviderId",
                schema: "mwp",
                table: "CloudServices",
                column: "CloudServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudServices_CloudServiceTypeId",
                schema: "mwp",
                table: "CloudServices",
                column: "CloudServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IDX_FORM_HIERARCHICAL_PATH",
                schema: "mwp",
                table: "Forms",
                column: "HierarchicalPath");

            migrationBuilder.CreateIndex(
                name: "IDX_FORM_PARENT_ID",
                schema: "mwp",
                table: "Forms",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedResources_CloudServiceLocationId",
                schema: "mwp",
                table: "SharedResources",
                column: "CloudServiceLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedResources_CloudServiceOptionId",
                schema: "mwp",
                table: "SharedResources",
                column: "CloudServiceOptionId");

            migrationBuilder.CreateIndex(
                name: "IDX_SUBMISSION_FORM_ID",
                schema: "mwp",
                table: "Submissions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantResources_CloudServiceLocationId",
                schema: "mwp",
                table: "TenantResources",
                column: "CloudServiceLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantResources_CloudServiceOptionId",
                schema: "mwp",
                table: "TenantResources",
                column: "CloudServiceOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantResources_TenantId",
                schema: "mwp",
                table: "TenantResources",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IDX_WOPIHISTORIES_WOPI_FILE_ID",
                schema: "mwp",
                table: "WopiFileHistories",
                column: "WopiFileId");

            migrationBuilder.CreateIndex(
                name: "IDX_XEROCONNECTIONS_MWP_USER_ID",
                schema: "mwp",
                table: "XeroConnections",
                column: "MwpUserId");

            migrationBuilder.CreateIndex(
                name: "IX_XeroConnections_XeroTenantId",
                schema: "mwp",
                table: "XeroConnections",
                column: "XeroTenantId");

            migrationBuilder.CreateIndex(
                name: "IDX_XEROTENANTS_MWP_TENANT_ID",
                schema: "mwp",
                table: "XeroTenants",
                column: "MwpTenantId");

            migrationBuilder.CreateIndex(
                name: "IDX_XEROTOKENS_MWP_USER_ID",
                schema: "mwp",
                table: "XeroTokens",
                column: "MwpUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedResources",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "Submissions",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "TenantResources",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "UserFormConfigs",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "WopiFileHistories",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "XeroConnections",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "XeroTokens",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "Forms",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "CloudServiceLocations",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "CloudServiceOptions",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "WopiFiles",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "XeroTenants",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "CloudServices",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "CloudServiceProviders",
                schema: "mwp");

            migrationBuilder.DropTable(
                name: "CloudServiceTypes",
                schema: "mwp");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SaasTenants");

            migrationBuilder.DropColumn(
                name: "TenantParentId",
                table: "SaasTenants");
        }
    }
}
