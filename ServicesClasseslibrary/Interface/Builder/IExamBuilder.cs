using DataModel;
using ServicesClasseslibrary.Implmentation.Builder;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ServicesClasseslibrary.Interface.Builder
{
    public interface IExamBuilder
    {

       public ExamAttempt Build(int ExamId);
    }
    public class ExamBuilder : IExamBuilder
    {




       
        private readonly IExamSectionService _examSectionService;
        private readonly IExamAttemptQuestionBuilder _examAttemptQuestionBuilder;



        public ExamBuilder(IExamSectionService examSectionService, IExamAttemptQuestionBuilder examAttemptQuestionBuilder)
        {
            _examSectionService = examSectionService;
            _examAttemptQuestionBuilder = examAttemptQuestionBuilder;
        }

        public ExamAttempt Build(int ExamId)
        {
            //READ FROM DATABASE SECTIONS OF EXAM
             var sections  = _examSectionService.List(ExamId);
             int studySubjectId = sections.First().exam.StudySubjectId; 
            var rules = sections.SelectMany(s=>s.ExamSectionRulesDataModel).ToList();  
            ExamAttempt examAttempt = new ExamAttempt();
            rules.ForEach(rule =>
             {
                examAttempt.Questions.AddRange(_examAttemptQuestionBuilder.BuildExamAttemptQuestions(studySubjectId, rule));
             });
         


            return examAttempt;




        }
    }
}




