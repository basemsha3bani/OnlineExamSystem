using DataModel;
using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    public interface IExamAttemptOprations
    {
        int Add(ExamAttemptDataModel model);
        ExamAttemptDataModel CheckIfAttemptSubmitted(int id);
        void Edit(ExamAttemptDataModel model);
        ExamAttemptDataModel GetById(int id);
        List<ExamAttemptDataModel> listByUser(int userId);
        List<ExamAttemptDataModel> listSubmitted();
    }
}
