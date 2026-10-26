using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary.Interface.DataRepository
{
   public interface IQuestionsOperations
    {
        void Add(QuestionsDTO questions);
        void Edit(QuestionsDTO questions);



        void Delete(int id);


        QuestionsDTO GetById(int id);

        QuestionPageDTO Search(QuestionSearchDTO search);
        List<QuestionsDTO> list();

        List<QuestionsDTO> GetRandomQuestions(int studySubjectId, ExamSectionRulesDTO rule);

    }
}
