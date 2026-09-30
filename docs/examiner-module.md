# Examiner module

Examiners sign in through AuthController and are redirected to `/Examiner`. They select an exam, answer its sections, and submit. Answers are saved on Previous, Next, or Save answers. In-progress attempts can be resumed from `/Examiner/Attempts`.

Submission atomically saves the answers and changes the persisted attempt status to `Submitted`. The thank-you page returns immediately. The application-hosted worker checks submitted attempts every five seconds while the application is running. It stores the correct count, total question count, score ratio, and evaluation timestamp, then sets `Scored`. Pending attempts survive restarts; errors are logged and retried without blocking other attempts. The status page has a Refresh status link and completed results remain available.

Questions, section names, answer options, and the answer key are stored as a JSON snapshot in the new `ExaminerAttempts` table. Existing question-answer tables remain the source when starting an exam. Correctness is never included in the question view model. Later question edits do not change existing attempts. A rowversion protects concurrent saves and evaluation. Repeated submission does not alter a finished attempt.

The score is `correct / total` using decimal division. Blank answers are incorrect. TotalMarks and section percentages are not used. Empty configurations, insufficient unique questions, and questions without exactly one correct option prevent an attempt from starting.

## Database setup

The migration `20260930105742_AddExaminerAttempts` creates only the attempt table and its indexes. Apply it to the configured application database before running this version. Automatic migration on application startup is intentionally not enabled.

The historical migrations in this repository lag behind the current admin schema (Exams, ExamSections, ExamSectionRules, Users, and StudySubjects). The new migration preserves that history rather than dropping or recreating existing tables. An existing database must already have the schema used by the current admin module. Rebuilding the complete admin schema from the historical migrations is outside this change.

For an existing database with its migration history already current:

```powershell
dotnet ef database update --project DataRepository --startup-project OnlineExamSystem
```

To review SQL for only this new migration:

```powershell
dotnet ef migrations script 20210414104312_mFixExamQuestionsSchema 20260930105742_AddExaminerAttempts --project DataRepository --startup-project OnlineExamSystem
```

## Verification

```powershell
dotnet build OnlineExamSystem.sln
dotnet run --project tests/ExaminerFlowChecks/ExaminerFlowChecks.csproj
```

The integration checks create and remove a uniquely named SQL Server LocalDB database. They do not use the application's connection string. They require the .NET 8 runtime and SQL Server LocalDB. Checks cover section generation, distinct questions, ownership, saved answers, option validation, duplicate submission, durable pending status, answer-key snapshots, scoring, repeated evaluation, and insufficient question pools.

