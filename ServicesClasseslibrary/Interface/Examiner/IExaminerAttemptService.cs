using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.Examiner
{
    public interface IExaminerAttemptService
    {
        int Start(int examId, int userId);
        ExamAttemptDataModel Get(int id);
        List<ExamAttemptDataModel> List(int userId);
        void Save(int id, int sectionIndex, Dictionary<int, int?> answers, bool submit);
        void EvaluatePending(Action<int, Exception> onFailure = null);
    }
}
