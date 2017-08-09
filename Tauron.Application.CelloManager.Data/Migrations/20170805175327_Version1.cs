using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tauron.Application.CelloManager.Data.Migrations
{
    public partial class Version1 : Migration
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
                    SentTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommittedRefills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommittedSpool",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommittedRefillId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrderedCount = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommittedSpool", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommittedSpool_CommittedRefills_CommittedRefillId",
                        column: x => x.CommittedRefillId,
                        principalTable: "CommittedRefills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommittedSpool_CommittedRefillId",
                table: "CommittedSpool",
                column: "CommittedRefillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CelloSpools");

            migrationBuilder.DropTable(
                name: "CommittedSpool");

            migrationBuilder.DropTable(
                name: "CommittedRefills");
        }
    }
}
