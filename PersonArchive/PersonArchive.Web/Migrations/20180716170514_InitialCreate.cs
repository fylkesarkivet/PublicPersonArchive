using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PersonArchive.Web.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    PersonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Gender = table.Column<int>(nullable: false, defaultValue: 4),
                    PersonGuid = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    TimeLastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "PersonDescription",
                columns: table => new
                {
                    PersonDescriptionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    TimeLastUpdated = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonDescription", x => x.PersonDescriptionId);
                    table.ForeignKey(
                        name: "FK_PersonDescription_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonFluffyDate",
                columns: table => new
                {
                    PersonFluffyDateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Day = table.Column<int>(nullable: true),
                    Month = table.Column<int>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    TimeLastUpdated = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonFluffyDate", x => x.PersonFluffyDateId);
                    table.ForeignKey(
                        name: "FK_PersonFluffyDate_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonName",
                columns: table => new
                {
                    PersonNameId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    First = table.Column<string>(nullable: true),
                    Last = table.Column<string>(nullable: true),
                    Middle = table.Column<string>(nullable: true),
                    NameWeight = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    PersonNameGuid = table.Column<Guid>(nullable: false),
                    Prefix = table.Column<string>(nullable: true),
                    Suffix = table.Column<string>(nullable: true),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    TimeLastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonName", x => x.PersonNameId);
                    table.ForeignKey(
                        name: "FK_PersonName_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Person_PersonGuid",
                table: "Person",
                column: "PersonGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonDescription_PersonId",
                table: "PersonDescription",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonFluffyDate_PersonId",
                table: "PersonFluffyDate",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonName_PersonId",
                table: "PersonName",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonName_PersonNameGuid",
                table: "PersonName",
                column: "PersonNameGuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonDescription");

            migrationBuilder.DropTable(
                name: "PersonFluffyDate");

            migrationBuilder.DropTable(
                name: "PersonName");

            migrationBuilder.DropTable(
                name: "Person");
        }
    }
}
