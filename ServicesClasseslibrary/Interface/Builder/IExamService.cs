using DataModel;
using System;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public interface IExamService
    {
        List<ExamDataModel> List();
        ExamDataModel GetById(int id);
        void Add(ExamDataModel model);
        void Edit(ExamDataModel model);
        List<ExamSectionRulesDataModel> GetSectionRules(int sectionId);
        void AddSectionRule(ExamSectionRulesDataModel model);
        ExamSectionsDataModel GetSectionById(int sectionId);
    }
}

