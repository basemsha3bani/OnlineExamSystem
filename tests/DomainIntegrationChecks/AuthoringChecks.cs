using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
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
        var exam = ExamDTO.Define( Title: "Separate creation", StudySubjectId: 1 );
        exams.Add(exam);
        if (exam.Id != 1 || exam.Sections.Count != 0) throw new Exception("Exam must save without sections");
        var section = ExamSectionsDTO.Define( ExamId: exam.Id, SectionName: "Section 1" );
        sections.Add(section);
        if (section.Id != 1 || section.NumberOfQuestions != 0 || section.ExamSectionRules.Count != 0) throw new Exception("Section must save by name alone");
        sections.Edit(ExamSectionsDTO.Define( Id: section.Id, SectionName: section.SectionName, NumberOfQuestions: 5, ChapterNumbers: new() { 1 } ));
        exams.AddSectionRule(ExamSectionRulesDTO.Define( SectionId: section.Id, DifficultyLevelId: 2, NoOfQuestions: 3 ));
        exams.AddSectionRule(ExamSectionRulesDTO.Define( SectionId: section.Id, DifficultyLevelId: 3, NoOfQuestions: 2 ));
        if (exams.GetSectionRules(section.Id).Sum(r => r.NoOfQuestions) != 5) throw new Exception("Rules did not complete the section");
        Reject(() => exams.EditSectionRule(ExamSectionRulesDTO.Define( Id: 2, DifficultyLevelId: 3, NoOfQuestions: 3 )));
        Reject(() => exams.EditSectionRule(ExamSectionRulesDTO.Define( Id: 2, DifficultyLevelId: 2, NoOfQuestions: 2 )));
        exams.EditSectionRule(ExamSectionRulesDTO.Define( Id: 2, SectionId: 999, DifficultyLevelId: 1, NoOfQuestions: 2 ));
        if (exams.GetSectionRule(2).SectionId != section.Id || exams.GetSectionRule(2).DifficultyLevelId != 1) throw new Exception("Rule edit lost its parent or difficulty");
        Reject(() => sections.Edit(ExamSectionsDTO.Define( Id: section.Id, SectionName: section.SectionName, NumberOfQuestions: 4, ChapterNumbers: new() { 1 } )));
        Console.WriteLine("Passed staged exam/section creation, rule addition/editing, parent preservation, duplicate/over-allocation validation.");
    }
    static void Reject(Action action)
    {
        try { action(); } catch (DomainException) { return; }
        throw new Exception("Expected a domain validation failure");
    }
    sealed class Store : IExamOprations, IExamSectionsOperations, IExamSectionRuleOperations, IStudySubjectsOperations
    {
        ExamDTO exam;
        ExamSectionsDTO section;
        readonly List<ExamSectionRulesDTO> rules = new();
        void IExamOprations.Add(ExamDTO model) { model.ChangeId(1); exam = model; }
        void IExamOprations.Edit(ExamDTO model) => exam = model;
        ExamDTO IExamOprations.GetById(int id) => id == exam?.Id ? exam : null;
        List<ExamDTO> IExamOprations.list() => new() { exam };
        void IExamSectionsOperations.Add(ExamSectionsDTO model) { model.ChangeId(1); section = model; }
        void IExamSectionsOperations.Edit(ExamSectionsDTO model) { section = model; section.ChangeExamSectionRules(rules); }
        ExamSectionsDTO IExamSectionsOperations.GeById(int id) => id == section?.Id ? section : null;
        List<ExamSectionsDTO> IExamSectionsOperations.List(int id) => section == null ? new() : new() { section };
        static ExamSectionRulesDTO Copy(ExamSectionRulesDTO m) => ExamSectionRulesDTO.Define( Id: m.Id, SectionId: m.SectionId, DifficultyLevelId: m.DifficultyLevelId, NoOfQuestions: m.NoOfQuestions );
        List<ExamSectionRulesDTO> IExamSectionRuleOperations.GetBySectionId(int id) => rules.Where(r => r.SectionId == id).Select(Copy).ToList();
        ExamSectionRulesDTO IExamSectionRuleOperations.GetById(int id) => rules.Where(r => r.Id == id).Select(Copy).SingleOrDefault();
        void IExamSectionRuleOperations.Add(ExamSectionRulesDTO model) { model.ChangeId(rules.Count + 1); rules.Add(Copy(model)); }
        void IExamSectionRuleOperations.Edit(ExamSectionRulesDTO model) { rules[rules.FindIndex(r => r.Id == model.Id)] = Copy(model); }
        StudySubjectDTO IStudySubjectsOperations.GetById(int id) => StudySubjectDTO.Define( Id: 1, SubjectName: "Subject", NumberOfChapters: 10 );
        List<StudySubjectDTO> IStudySubjectsOperations.list() => new();
        void IStudySubjectsOperations.Add(StudySubjectDTO model) => throw new NotSupportedException();
        void IStudySubjectsOperations.Edit(StudySubjectDTO model) => throw new NotSupportedException();
        void IStudySubjectsOperations.Delete(int id) => throw new NotSupportedException();
    }
}
