using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mwp.Migrations
{
    public partial class PDFTron : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PdfAnnotations",
                schema: "mwp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true),
                    FileId = table.Column<Guid>(maxLength: 36, nullable: false),
                    AnnotationId = table.Column<Guid>(maxLength: 36, nullable: false),
                    Annotation = table.Column<string>(nullable: false),
                    IsDiscarded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdfAnnotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PdfAnnotations_SaasTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "SaasTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_PDFANNOTATION_MWP_ANNOTATION_ID",
                schema: "mwp",
                table: "PdfAnnotations",
                column: "AnnotationId");

            migrationBuilder.CreateIndex(
                name: "IDX_PDFANNOTATION_MWP_FILE_ID",
                schema: "mwp",
                table: "PdfAnnotations",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_PdfAnnotations_TenantId",
                schema: "mwp",
                table: "PdfAnnotations",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PdfAnnotations",
                schema: "mwp");
        }
    }
}
