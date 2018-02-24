using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Tauron.Application.CelloManager.Data.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CelloSpools",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Neededamount = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CelloSpools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommittedRefills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompledTime = table.Column<DateTime>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    SentTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommittedRefills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(nullable: true),
                    key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommittedSpoolEntity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommittedRefillEntityId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrderedCount = table.Column<int>(nullable: false),
                    SpoolId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommittedSpoolEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommittedSpoolEntity_CommittedRefills_CommittedRefillEntityId",
                        column: x => x.CommittedRefillEntityId,
                        principalTable: "CommittedRefills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommittedSpoolEntity_CommittedRefillEntityId",
                table: "CommittedSpoolEntity",
                column: "CommittedRefillEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CelloSpools");

            migrationBuilder.DropTable(
                name: "CommittedSpoolEntity");

            migrationBuilder.DropTable(
                name: "OptionEntries");

            migrationBuilder.DropTable(
                name: "CommittedRefills");
        }
    }
}
