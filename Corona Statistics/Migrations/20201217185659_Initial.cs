using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Corona_Statistics.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FederalStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FederalStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TotalCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    PopulationsSum = table.Column<int>(type: "int", nullable: false),
                    CasesSum = table.Column<int>(type: "int", nullable: false),
                    DeathsSum = table.Column<int>(type: "int", nullable: false),
                    SevenDaysIncidentsSum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotalCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FederalStateId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Covid19CasesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_FederalStates_FederalStateId",
                        column: x => x.FederalStateId,
                        principalTable: "FederalStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Covid19Cases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    Population = table.Column<int>(type: "int", nullable: false),
                    Cases = table.Column<int>(type: "int", nullable: false),
                    Deaths = table.Column<int>(type: "int", nullable: false),
                    SevenDayIncidence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Covid19Cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Covid19Cases_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Covid19Cases_DistrictId",
                table: "Covid19Cases",
                column: "DistrictId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Districts_FederalStateId",
                table: "Districts",
                column: "FederalStateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Covid19Cases");

            migrationBuilder.DropTable(
                name: "TotalCases");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "FederalStates");
        }
    }
}
