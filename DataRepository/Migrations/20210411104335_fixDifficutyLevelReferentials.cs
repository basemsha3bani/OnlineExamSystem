using Microsoft.EntityFrameworkCore.Migrations;

namespace DataRepository.Migrations
{
    public partial class fixDifficutyLevelReferentials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevelId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_DifficultyLevelId",
                table: "Questions",
                column: "DifficultyLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTypesDetails_DifficultyLevelId",
                table: "ExamTypesDetails",
                column: "DifficultyLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamTypesDetails_DifficultyLevels_DifficultyLevelId",
                table: "ExamTypesDetails",
                column: "DifficultyLevelId",
                principalTable: "DifficultyLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_DifficultyLevels_DifficultyLevelId",
                table: "Questions",
                column: "DifficultyLevelId",
                principalTable: "DifficultyLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamTypesDetails_DifficultyLevels_DifficultyLevelId",
                table: "ExamTypesDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_DifficultyLevels_DifficultyLevelId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_DifficultyLevelId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_ExamTypesDetails_DifficultyLevelId",
                table: "ExamTypesDetails");

            migrationBuilder.DropColumn(
                name: "DifficultyLevelId",
                table: "Questions");
        }
    }
}
