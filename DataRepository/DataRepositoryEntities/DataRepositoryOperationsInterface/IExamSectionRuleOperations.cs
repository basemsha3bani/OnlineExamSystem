using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    namespace DataRepository.DataRepositoryEntities
    {
        public interface IExamSectionRuleOperations
        {
            List<ExamSectionRules> GetBySectionId(int sectionId);
            void Add(ExamSectionRules entity);
        }
    }


}
