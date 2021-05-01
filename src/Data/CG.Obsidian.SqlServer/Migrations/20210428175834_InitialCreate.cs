using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CG.Obsidian.SqlServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Obsidian");

            migrationBuilder.CreateTable(
                name: "MimeTypes",
                schema: "Obsidian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    SubType = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2021, 4, 28, 12, 58, 34, 324, DateTimeKind.Local).AddTicks(2844)),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MimeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileExtensions",
                schema: "Obsidian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MimeTypeId = table.Column<int>(type: "int", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2021, 4, 28, 12, 58, 34, 340, DateTimeKind.Local).AddTicks(9482)),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileExtensions", x => new { x.Id, x.MimeTypeId });
                    table.ForeignKey(
                        name: "FK_FileExtensions_MimeTypes_MimeTypeId",
                        column: x => x.MimeTypeId,
                        principalSchema: "Obsidian",
                        principalTable: "MimeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileExtensions_Extension",
                schema: "Obsidian",
                table: "FileExtensions",
                column: "Extension",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileExtensions_MimeTypeId",
                schema: "Obsidian",
                table: "FileExtensions",
                column: "MimeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MimeTypes_Type_SubType",
                schema: "Obsidian",
                table: "MimeTypes",
                columns: new[] { "Type", "SubType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileExtensions",
                schema: "Obsidian");

            migrationBuilder.DropTable(
                name: "MimeTypes",
                schema: "Obsidian");
        }
    }
}
