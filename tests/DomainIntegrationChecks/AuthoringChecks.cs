using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using Domain.Exceptions;
using ServicesClasseslibrary;
using ServicesClasseslibrary.Implmentation.Builder;
static class AuthoringChecks
{
    public static void Run()
    {
        var store = new Store();
        var exams = new ExamService(store, store, store, store);
        var sections = new ExamSectionService(store, store, store);
        var exam = new ExamDataModel { Title = "Separate creation", StudySubjectId = 1 };
        exams.Add(exam);
        if (exam.Id != 1 || exam.Sections.Count != 0) throw new Exception("Exam must save without sections");
        var section = new ExamSectionsDataModel { ExamId = exam.Id, SectionName = "Section 1" };
        sections.Add(section);
        if (section.Id != 1 || section.NumberOfQuestions != 0 || section.ExamSectionRules.Count != 0) throw new Exception("Section must save by name alone");
        sections.Edit(new() { Id = section.Id, SectionName = section.SectionName, NumberOfQuestions = 5, ChapterNumbers = new() { 1 } });
        exams.AddSectionRule(new() { SectionId = section.Id, DifficultyLevelId = 2, NoOfQuestions = 3 });
        exams.AddSectionRule(new() { SectionId = section.Id, DifficultyLevelId = 3, NoOfQuestions = 2 });
        if (exams.GetSectionRules(section.Id).Sum(r => r.NoOfQuestions) != 5) throw new Exception("Rules did not complete the section");
        Reject(() => exams.EditSectionRule(new() { Id = 2, DifficultyLevelId = 3, NoOfQuestions = 3 }));
        Reject(() => exams.EditSectionRule(new() { Id = 2, DifficultyLevelId = 2, NoOfQuestions = 2 }));
        exams.EditSectionRule(new() { Id = 2, SectionId = 999, DifficultyLevelId = 1, NoOfQuestions = 2 });
        if (exams.GetSectionRule(2).SectionId != section.Id || exams.GetSectionRule(2).DifficultyLevelId != 1) throw new Exception("Rule edit lost its parent or difficulty");
        Reject(() => sections.Edit(new() { Id = section.Id, SectionName = section.SectionName, NumberOfQuestions = 4, ChapterNumbers = new() { 1 } }));
        Console.WriteLine("Passed staged exam/section creation, rule addition/editing, parent preservation, duplicate/over-allocation validation.");
    }
    static void Reject(Action action)
    {
        try { action(); } catch (DomainException) { return; }
        throw new Exception("Expected a domain validation failure");
    }
    sealed class Store : IExamOprations, IExamSectionsOperations, IExamSectionRuleOperations, IStudySubjectsOperations
    {
        ExamDataModel exam;
        ExamSectionsDataModel section;
        readonly List<ExamSectionRulesDataModel> rules = new();
        void IExamOprations.Add(ExamDataModel model) { model.Id = 1; exam = model; }
        void IExamOprations.Edit(ExamDataModel model) => exam = model;
        ExamDataModel IExamOprations.GetById(int id) => id == exam?.Id ? exam : null;
        List<ExamDataModel> IExamOprations.list() => new() { exam };
        void IExamSectionsOperations.Add(ExamSectionsDataModel model) { model.Id = 1; section = model; }
        void IExamSectionsOperations.Edit(ExamSectionsDataModel model) { section = model; section.ExamSectionRules = rules; }
        ExamSectionsDataModel IExamSectionsOperations.GeById(int id) => id == section?.Id ? section : null;
        List<ExamSectionsDataModel> IExamSectionsOperations.List(int id) => section == null ? new() : new() { section };
        static ExamSectionRulesDataModel Copy(ExamSectionRulesDataModel m) => new() { Id = m.Id, SectionId = m.SectionId, DifficultyLevelId = m.DifficultyLevelId, NoOfQuestions = m.NoOfQuestions };
        List<ExamSectionRulesDataModel> IExamSectionRuleOperations.GetBySectionId(int id) => rules.Where(r => r.SectionId == id).Select(Copy).ToList();
        ExamSectionRulesDataModel IExamSectionRuleOperations.GetById(int id) => rules.Where(r => r.Id == id).Select(Copy).SingleOrDefault();
        void IExamSectionRuleOperations.Add(ExamSectionRulesDataModel model) { model.Id = rules.Count + 1; rules.Add(Copy(model)); }
        void IExamSectionRuleOperations.Edit(ExamSectionRulesDataModel model) { rules[rules.FindIndex(r => r.Id == model.Id)] = Copy(model); }
        StudySubjectDataModel IStudySubjectsOperations.GetById(int id) => new() { Id = 1, SubjectName = "Subject", NumberOfChapters = 10 };
        List<StudySubjectDataModel> IStudySubjectsOperations.list() => new();
        void IStudySubjectsOperations.Add(StudySubjectDataModel model) => throw new NotSupportedException();
        void IStudySubjectsOperations.Edit(StudySubjectDataModel model) => throw new NotSupportedException();
        void IStudySubjectsOperations.Delete(int id) => throw new NotSupportedException();
    }
}
