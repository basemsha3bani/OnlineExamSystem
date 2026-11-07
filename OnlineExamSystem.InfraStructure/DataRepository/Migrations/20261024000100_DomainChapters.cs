using DataRepository.GateWay;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace DataRepository.Migrations;
[DbContext(typeof(DbConext))]
[Migration("20261024000100_DomainChapters")]
public sealed partial class DomainChapters : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>("NumberOfChapters", "StudySubjects", type: "int", nullable: false, defaultValue: 1);
        migrationBuilder.AddColumn<int>("ChapterNumber", "Questions", type: "int", nullable: false, defaultValue: 1);
        migrationBuilder.AddColumn<int>("NumberOfQuestions", "ExamSections", type: "int", nullable: false, defaultValue: 0);
        migrationBuilder.AddColumn<string>("ChapterNumbers", "ExamSections", type: "nvarchar(max)", nullable: false, defaultValue: "[1]");
        migrationBuilder.Sql("UPDATE s SET NumberOfQuestions = COALESCE((SELECT SUM(r.NoOfQuestions) FROM ExamSectionRules r WHERE r.SectionId = s.Id), 0) FROM ExamSections s");
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("NumberOfChapters", "StudySubjects");
        migrationBuilder.DropColumn("ChapterNumber", "Questions");
        migrationBuilder.DropColumn("NumberOfQuestions", "ExamSections");
        migrationBuilder.DropColumn("ChapterNumbers", "ExamSections");
    }
}
