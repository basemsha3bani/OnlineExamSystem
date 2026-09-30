using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    namespace DataRepository.DataRepositoryEntities
    {
        public interface IExamSectionRuleOperations
        {
            List<DataModel.ExamSectionRulesDataModel> GetBySectionId(int sectionId);
            void Add(DataModel.ExamSectionRulesDataModel model);
        }
    }


}
