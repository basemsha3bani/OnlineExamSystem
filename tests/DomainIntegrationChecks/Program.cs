using AutoMapper;
using DataRepository.GateWay;
using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
AuthoringChecks.Run();
await BrokerResilienceChecks.RunAsync();
var mapping = new MapperConfiguration(cfg => cfg.AddProfile<DataRepository.ModelMapper.RepositoryMappingProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
mapping.AssertConfigurationIsValid();
var mapper = mapping.CreateMapper();
if (args.Contains("--localdb")) ExamAttemptBuilderDatabaseChecks.Run(mapper);
var dto = mapper.Map<DataModel.ExamSectionsDataModel>(new ExamSections { Id = 1, NumberOfQuestions = 5, ChapterNumbers = new() { 1, 3 } });
var entity = mapper.Map<ExamSections>(dto);
if (entity.NumberOfQuestions != 5 || !entity.ChapterNumbers.SequenceEqual(new[] { 1, 3 })) throw new Exception("Section mapping lost domain configuration");
if (mapper.Map<DataModel.StudySubjectDataModel>(new StudySubject { NumberOfChapters = 10 }).NumberOfChapters != 10) throw new Exception("Subject mapping lost chapters");
if (mapper.Map<DataModel.QuestionsDataModel>(new Questions { ChapterNumber = 7 }).ChapterNumber != 7) throw new Exception("Question mapping lost chapter");
var previous = Directory.GetCurrentDirectory();
var temporary = Path.Combine(Path.GetTempPath(), "DomainIntegrationChecks_" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(temporary);
try
{
    File.WriteAllText(Path.Combine(temporary, "appsettings.json"), "{\"ConnectionStrings\":{\"DbCoreConnectionString\":\"Server=(localdb)\\\\MSSQLLocalDB;Database=UnusedDomainChecks;Integrated Security=true\"}}");
    Directory.SetCurrentDirectory(temporary);
    using var db = new DbConext(new DbContextOptionsBuilder<DbConext>().Options, new DataBaseConnectionConfiguration());
    QuestionListingChecks.Run(db);
    var chapters = db.Model.FindEntityType(typeof(ExamSections))!.FindProperty("ChapterNumbers");
    if (chapters == null || !chapters.IsPrimitiveCollection) throw new Exception("Section chapters are not persisted");
    var sql = db.GetService<IMigrator>().GenerateScript("20261006040614_MPerformanceTable", "20261024000100_DomainChapters");
    if (!sql.Contains("ChapterNumbers") || !sql.Contains("NumberOfChapters") || !sql.Contains("ChapterNumber") || !sql.Contains("UPDATE s SET NumberOfQuestions")) throw new Exception("Migration script incomplete");
    var removedPointsSql = db.GetService<IMigrator>().GenerateScript("20261024000100_DomainChapters", "20261024000200_RemoveSectionPoints");
    if (!removedPointsSql.Contains("DROP COLUMN [Percentage]")) throw new Exception("Section points were not removed");
    if (db.Model.FindEntityType(typeof(ExamSections))!.FindProperty("Percentage") != null) throw new Exception("Section points remain in the model");
    if (DataModel.AttemptScoring.Ratio(3, 5) != 0.6m) throw new Exception("Scoring must be correct answers divided by total questions");
    var rollback = db.GetService<IMigrator>().GenerateScript("20261024000100_DomainChapters", "20261006040614_MPerformanceTable");
    if (!rollback.Contains("DROP COLUMN")) throw new Exception("Rollback script incomplete");
    using var graph = new CapturingContext();
    var definition = new DataModel.ExamSectionsDataModel { SectionName = "Section 1", NumberOfQuestions = 5, ChapterNumbers = new() { 1 },
        ExamSectionRules = new() {
            new() { DifficultyLevelId = 1, NoOfQuestions = 0 },
            new() { DifficultyLevelId = 2, NoOfQuestions = 3 },
            new() { DifficultyLevelId = 3, NoOfQuestions = 2 } } };
    var repository = new ExamsOperations(new ContextGateway<Exams>(graph), mapper);
    repository.Add(new DataModel.ExamDataModel { Title = "Distribution", StudySubjectId = 1, Sections = new() { definition } });
    var saved = graph.ChangeTracker.Entries<ExamSectionRules>().Select(e => e.Entity).OrderBy(r => r.DifficultyLevelId).ToList();
    if (saved.Count != 2 || saved[0].NoOfQuestions != 3 || saved[1].NoOfQuestions != 2 || saved.Any(r => r.section == null)) throw new Exception("Exam creation discarded or detached difficulty rules");
    using var sectionGraph = new CapturingContext();
    definition.ExamId = 1;
    new ExamSectionsOperations(new ContextGateway<ExamSections>(sectionGraph), mapper).Add(definition);
    if (sectionGraph.ChangeTracker.Entries<ExamSectionRules>().Count() != 2) throw new Exception("Standalone section creation discarded difficulty rules");
    Console.WriteLine("Passed AutoMapper round trips, EF primitive collection mapping, forward/rollback migration SQL generation, and exam/section difficulty persistence graphs. No database connection opened.");
}
finally
{
    Directory.SetCurrentDirectory(previous);
    Directory.Delete(temporary, true);
}

sealed class CapturingContext : DbConext
{
    public CapturingContext() : base(new DbContextOptionsBuilder<DbConext>().Options, new DataBaseConnectionConfiguration()) { }
    public override int SaveChanges() => 0; // Inspect tracked persistence graphs without opening a database.
}
