using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    
        public interface IExamSectionRuleOperations
        {
            List<DataModel.ExamSectionRulesDataModel> GetBySectionId(int sectionId);
            void Add(DataModel.ExamSectionRulesDataModel model);
        }
    


}
