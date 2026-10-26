using System.Collections.Generic;

namespace ServicesClasseslibrary.Interface.DataRepository
{
    
        public interface IExamSectionRuleOperations
        {
            List<Domain.DTOs.ExamSectionRulesDTO> GetBySectionId(int sectionId);
            Domain.DTOs.ExamSectionRulesDTO GetById(int id);
            void Edit(Domain.DTOs.ExamSectionRulesDTO model);
            void Add(Domain.DTOs.ExamSectionRulesDTO model);
        }
    


}
