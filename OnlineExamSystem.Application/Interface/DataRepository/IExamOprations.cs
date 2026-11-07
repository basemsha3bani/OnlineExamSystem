using Domain.DTOs;
using System.Collections.Generic;
using System.Text;

namespace OnlineExamSystem.Application.Interface.DataRepository
{
   public interface IExamOprations
    {

        void Add(ExamDTO Exam);
        void Edit(ExamDTO model);



        


        ExamDTO GetById(int id);

        List<ExamDTO> list();
    }
}
