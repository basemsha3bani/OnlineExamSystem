using Domain.DTOs;
using System;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public interface IExamService
    {
        List<ExamDTO> List();
        ExamDTO GetById(int id);
        void Add(ExamDTO model);
        void Edit(ExamDTO model);
        List<ExamSectionRulesDTO> GetSectionRules(int sectionId);
        ExamSectionRulesDTO GetSectionRule(int id);
        void EditSectionRule(ExamSectionRulesDTO model);
        void AddSectionRule(ExamSectionRulesDTO model);
       
    }
}

