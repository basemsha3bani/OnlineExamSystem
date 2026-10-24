# Exam domain (V2)

`Domain` has no MVC, EF Core, or repository dependencies. Its three aggregate roots are `StudySubject`, `Question`, and `Exam`. Sections and their chapter/difficulty values belong to Exam. Collections and properties are immutable after construction. Difficulty IDs map to the existing configurable catalogue (Easy/Medium/Hard helpers use IDs 1/2/3).

Question construction validates chapters against the supplied subject. Exam construction validates every section chapter against its subject. Section construction requires positive counts and an exact difficulty total. Domain exceptions inherit InvalidOperationException for compatibility with existing error handling.

MVC DTOs and EF entities remain transport/persistence models. Authoring is staged: save an exam (name and subject), create a section (name only), edit the section to configure its total question count and chapters, then add/edit individual difficulty rules. Incomplete sections are drafts. Rule edits validate positive counts, unique difficulties and a total that does not exceed the section count. The complete aggregate enforces exact equality before an attempt starts. Subject chapter reductions validate existing questions and section chapters. Section points are removed; final scoring is correct answers divided by the total number of questions.

The attempt builder loads subject questions once, builds the aggregate, and calls ExamDomainService. Selection filters subject, chapters, and difficulty, returns sections in input order, and never reuses a question ID. Matching supports overlapping chapter filters without greedy false shortages. Existing answer snapshots and scoring stay in the application layer.

Run acceptance checks:

```powershell
dotnet run --project tests/DomainChecks/DomainChecks.csproj
dotnet run --project tests/DomainIntegrationChecks/DomainIntegrationChecks.csproj
```

Apply the schema migration before using the updated app:

```powershell
dotnet ef database update --project DataRepository --startup-project OnlineExamSystem
```

The migration assigns existing subjects/questions to chapter 1, enables chapter 1 for existing sections, and derives existing section totals from saved difficulty rules. Configure actual chapter counts and chapter choices after migration. No application database is modified by the acceptance checks.

Questions can be filtered by subject and chapter independently or together. The repository applies filters, a count, stable ID ordering, and database pagination (10/20/50 rows; default 20), without loading answer collections. Filter selections are retained in page links. Section configuration saves use redirect-backed success messages and validation/database failure alerts that retain submitted values.

The builder restricts the database pool to required difficulty IDs and chapter numbers. It loads questions with stable ID ordering and identity resolution before shuffling in memory; this prevents collection Include rows from producing repeated question IDs. Run the isolated SQL Server regression with `dotnet run --project tests/DomainIntegrationChecks/DomainIntegrationChecks.csproj -- --localdb` (requires LocalDB; creates and deletes a uniquely named test database).

RabbitMQ connections are lazy and asynchronous. Evaluation, publishing, notifications, and log consumption catch failures at worker boundaries and retry after 5 seconds. The publisher has its own channel with confirms and keeps failed messages in an in-memory queue; each consumer session owns and disposes a separate channel. Consumers are registered once per session, and exchange/queue setup is awaited. The existing durable topic exchange is retained. Host-wide exception ignoring is not enabled. RabbitMq settings use the ASP.NET configuration/environment providers: Host, Port, UserName, Password, and VirtualHost. Pending notifications/logs survive broker outages while the application runs, but the buffer is not a durable outbox; stored exam results remain in SQL Server.
