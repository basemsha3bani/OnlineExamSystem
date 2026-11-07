using DataRepository.GateWay;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace DataRepository.Migrations;
[DbContext(typeof(DbConext))]
[Migration("20261024000200_RemoveSectionPoints")]
public sealed partial class RemoveSectionPoints : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn("Percentage", "ExamSections");
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<decimal>("Percentage", "ExamSections", type: "decimal(18,2)", nullable: false, defaultValue: 0m);
}
