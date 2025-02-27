using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestorationBot.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    RestorationStep = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Sex = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SportsmenId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreTrainingReportData_HeartRate = table.Column<double>(type: "double precision", nullable: false),
                    PreTrainingReportData_BloodPressure = table.Column<double>(type: "double precision", nullable: false),
                    PostTrainingReportData_HeartRate = table.Column<double>(type: "double precision", nullable: false),
                    PostTrainingReportData_BloodPressure = table.Column<double>(type: "double precision", nullable: false),
                    TrainingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingReports_Users_SportsmenId",
                        column: x => x.SportsmenId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingReports_SportsmenId",
                table: "TrainingReports",
                column: "SportsmenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingReports");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
