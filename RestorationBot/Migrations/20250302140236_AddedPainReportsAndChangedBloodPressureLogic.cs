using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestorationBot.Migrations
{
    /// <inheritdoc />
    public partial class AddedPainReportsAndChangedBloodPressureLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreTrainingReportData_BloodPressure",
                table: "TrainingReports",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "PostTrainingReportData_BloodPressure",
                table: "TrainingReports",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateTable(
                name: "PainReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PainLevel = table.Column<int>(type: "integer", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PainReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PainReports_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PainReports_AuthorId",
                table: "PainReports",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PainReports");

            migrationBuilder.AlterColumn<double>(
                name: "PreTrainingReportData_BloodPressure",
                table: "TrainingReports",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "PostTrainingReportData_BloodPressure",
                table: "TrainingReports",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
