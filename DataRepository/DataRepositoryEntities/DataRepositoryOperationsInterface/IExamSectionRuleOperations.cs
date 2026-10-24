using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    
        public interface IExamSectionRuleOperations
        {
            List<DataModel.ExamSectionRulesDataModel> GetBySectionId(int sectionId);
            DataModel.ExamSectionRulesDataModel GetById(int id);
            void Edit(DataModel.ExamSectionRulesDataModel model);
            void Add(DataModel.ExamSectionRulesDataModel model);
        }
    


}
