using DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    public interface IExamAttemptOprations
    {
        int Add(ExamAttemptDataModel model);
        ExamAttemptDataModel CheckIfAttemptSubmitted(int id);
        Task Edit(ExamAttemptDataModel model);
        ExamAttemptDataModel GetById(int id);
        List<ExamAttemptDataModel> listByUser(int userId);
        List<ExamAttemptDataModel> listSubmitted();
    }
}
