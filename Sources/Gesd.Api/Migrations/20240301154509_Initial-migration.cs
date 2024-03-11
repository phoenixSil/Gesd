using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gesd.Api.Migrations
{
    public partial class Initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModify = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdDocument = table.Column<int>(type: "int", nullable: false),
                    FileSize = table.Column<double>(type: "float", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EncryptedUrlFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EncryptedUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptedUrlFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncryptedUrlFiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyStores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneratedKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedUrlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyStores_EncryptedUrlFiles_EncryptedUrlId",
                        column: x => x.EncryptedUrlId,
                        principalTable: "EncryptedUrlFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncryptedUrlFiles_FileId",
                table: "EncryptedUrlFiles",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KeyStores_EncryptedUrlId",
                table: "KeyStores",
                column: "EncryptedUrlId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyStores");

            migrationBuilder.DropTable(
                name: "EncryptedUrlFiles");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
