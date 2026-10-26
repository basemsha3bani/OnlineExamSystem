using Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.DataRepository
{
    public interface IExamAttemptOprations
    {
        int Add(ExamAttemptDTO model);
        ExamAttemptDTO CheckIfAttemptSubmitted(int id);
        Task Edit(ExamAttemptDTO model);
        ExamAttemptDTO GetById(int id);
        List<ExamAttemptDTO> listByUser(int userId);
        List<ExamAttemptDTO> listSubmitted();
    }
}
