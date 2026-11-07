# Performance analysis summary

Candidate analytics are grouped by subject across all attempts, including repeated attempts of the same exam and different exams of the same subject. Each scored attempt contributes equally to the average. For example, Math scores of 60%, 70%, and 80% produce 70% across three scored attempts; Physics at 90% produces 90% across one.

## Data and migration

The aggregate table is ExaminerPerformance. UserId is the existing candidate identifier (matching ExaminerAttempts.UserId); no duplicate CandidateId column is needed. SubjectId is an int matching StudySubjects.Id. The primary key is (UserId, SubjectId), with a subject foreign key.

Migration Add_SubjectId_To_Aggregate rebuilds summaries from ExaminerAttempts joined to Exams. The legacy table contains no ExamId and a row can span multiple subjects, so a direct assignment of one subject to each old row would be incorrect. The rebuild splits those rows by candidate and subject and includes all existing attempts. With no attempts there is nothing to backfill. Attempts whose exam no longer exists cannot be assigned to a subject and are excluded by the join.

Apply with:

    dotnet ef database update --project OnlineExamSystem.InfraStructure --startup-project OnlineExamSystem

Rollback consolidates subject rows into candidate summaries, weighting averages and pass rates by scored attempt count.

## Worker and query

AnalyticsWorker continues consuming candidate jobs through IAnalyticsService. The repository recalculates every subject for that candidate, replacing totals rather than incrementing them, so replaying a job does not count an attempt twice. Existing queue delivery and retry behavior is unchanged.

ExaminerController.Analytics uses the session candidate identifier and accepts an optional subjectId. The view supplies an All subjects option and subject selection and a summary table. Subject choices come from the candidate's available summaries. Summaries update after background evaluation; averages, extrema, and pass rates exclude unscored attempts. Total attempts include pending and in-progress attempts; pass rate uses the existing 50% threshold.

## Verification

The web project builds with Razor compilation. Focused checks can be run with:

    dotnet run --project tests/DomainChecks -- --analytics-only
    dotnet run --project tests/DomainIntegrationChecks -- --analytics-only

The focused integration mode excludes the existing logging DI check and the unrelated pending OutboxMessages table from schema comparison. The full domain suite currently stops on the existing public OutboxMessage.Id setter check. Migration SQL is generated and checked; applying it to a live database requires the normal deployment step.
