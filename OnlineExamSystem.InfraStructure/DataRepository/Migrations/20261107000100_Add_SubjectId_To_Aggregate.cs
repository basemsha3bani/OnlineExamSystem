using Microsoft.EntityFrameworkCore.Migrations;
namespace DataRepository.Migrations;
public partial class Add_SubjectId_To_Aggregate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey("PK_ExaminerPerformance", "ExaminerPerformance");
        migrationBuilder.AddColumn<int>("SubjectId", "ExaminerPerformance", type: "int", nullable: false, defaultValue: 0);
        // A legacy candidate summary can span several exams/subjects. Rebuild from the
        // authoritative attempts instead of assigning that summary to just one subject.
        migrationBuilder.Sql(@"
DELETE FROM [ExaminerPerformance];
INSERT INTO [ExaminerPerformance]
([UserId], [SubjectId], [TotalAttempts], [EvaluatedCount], [InProgressCount],
 [AverageScore], [HighestScore], [LowestScore], [PassRate], [LastCalculatedAt], [LastCalculatedAttemptId])
SELECT a.UserId, e.StudySubjectId, COUNT(*),
 SUM(CASE WHEN a.Status = 'Scored' AND a.Score IS NOT NULL THEN 1 ELSE 0 END),
 SUM(CASE WHEN a.Status = 'InProgress' THEN 1 ELSE 0 END),
 COALESCE(AVG(CASE WHEN a.Status = 'Scored' THEN CAST(a.Score AS float) END) * 100, 0),
 COALESCE(MAX(CASE WHEN a.Status = 'Scored' THEN CAST(a.Score AS float) END) * 100, 0),
 COALESCE(MIN(CASE WHEN a.Status = 'Scored' THEN CAST(a.Score AS float) END) * 100, 0),
 COALESCE(100.0 * SUM(CASE WHEN a.Status = 'Scored' AND a.Score >= 0.5 THEN 1 ELSE 0 END)
 / NULLIF(SUM(CASE WHEN a.Status = 'Scored' AND a.Score IS NOT NULL THEN 1 ELSE 0 END), 0), 0),
 SYSUTCDATETIME(), MAX(a.Id)
FROM ExaminerAttempts a INNER JOIN Exams e ON a.ExamId = e.Id
GROUP BY a.UserId, e.StudySubjectId;");
        migrationBuilder.AddPrimaryKey("PK_ExaminerPerformance", "ExaminerPerformance", new[] { "UserId", "SubjectId" });
        migrationBuilder.CreateIndex("IX_ExaminerPerformance_SubjectId", "ExaminerPerformance", "SubjectId");
        migrationBuilder.AddForeignKey("FK_ExaminerPerformance_StudySubjects_SubjectId", "ExaminerPerformance", "SubjectId", "StudySubjects", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("FK_ExaminerPerformance_StudySubjects_SubjectId", "ExaminerPerformance");
        migrationBuilder.DropIndex("IX_ExaminerPerformance_SubjectId", "ExaminerPerformance");
        migrationBuilder.DropPrimaryKey("PK_ExaminerPerformance", "ExaminerPerformance");
        migrationBuilder.Sql(@"
SELECT UserId, SUM(TotalAttempts) TotalAttempts, SUM(EvaluatedCount) EvaluatedCount,
 SUM(InProgressCount) InProgressCount,
 COALESCE(SUM(AverageScore * EvaluatedCount) / NULLIF(SUM(EvaluatedCount), 0), 0) AverageScore,
 COALESCE(MAX(CASE WHEN EvaluatedCount > 0 THEN HighestScore END), 0) / 100 HighestScore,
 COALESCE(MIN(CASE WHEN EvaluatedCount > 0 THEN LowestScore END), 0) / 100 LowestScore,
 COALESCE(SUM(PassRate * EvaluatedCount) / NULLIF(SUM(EvaluatedCount), 0), 0) PassRate,
 MAX(LastCalculatedAt) LastCalculatedAt, MAX(LastCalculatedAttemptId) LastCalculatedAttemptId
INTO #PerformanceRollback FROM ExaminerPerformance GROUP BY UserId;
DELETE FROM ExaminerPerformance;
INSERT INTO ExaminerPerformance
(UserId, SubjectId, TotalAttempts, EvaluatedCount, InProgressCount, AverageScore, HighestScore, LowestScore, PassRate, LastCalculatedAt, LastCalculatedAttemptId)
SELECT UserId, 0, TotalAttempts, EvaluatedCount, InProgressCount, AverageScore, HighestScore, LowestScore, PassRate, LastCalculatedAt, LastCalculatedAttemptId FROM #PerformanceRollback;
DROP TABLE #PerformanceRollback;");
        migrationBuilder.DropColumn("SubjectId", "ExaminerPerformance");
        migrationBuilder.AddPrimaryKey("PK_ExaminerPerformance", "ExaminerPerformance", "UserId");
    }
}
