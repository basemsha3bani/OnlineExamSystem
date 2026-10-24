using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using ServicesClasseslibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServicesClasseslibrary
{
    public class QuestionsService : IQuestionsService
    {
        private readonly IQuestionsOperations _questionsOperations;
        private readonly IStudySubjectsOperations subjects;
      
        public  QuestionsService(IQuestionsOperations questionsOperations, IStudySubjectsOperations subjects)
        {
            _questionsOperations = questionsOperations;
            this.subjects = subjects;
           
        }
        public void Add(QuestionsDataModel question)
        {
            Validate(question);
            _questionsOperations.Add(question);
        }

        private void Validate(QuestionsDataModel model)
        {
            var subject = model.StudySubjectId.HasValue ? subjects.GetById(model.StudySubjectId.Value) : null;
            if (subject == null) throw new Domain.Exceptions.DomainException("Subject is required.");
            _ = new Domain.Aggregates.Question(model.Id, new Domain.Aggregates.StudySubject(subject.Id, subject.SubjectName, subject.NumberOfChapters), model.ChapterNumber, new Domain.Aggregates.Difficulty(model.DifficultyLevelId));
        }

        public void Delete(int id)
        {
           
        }

        public void Edit(QuestionsDataModel questionAnswers)
        {
            Validate(questionAnswers);
            _questionsOperations.Edit(questionAnswers);
        }

        public QuestionsDataModel GetById(int id)
        {
          return _questionsOperations.GetById(id);
        }

        public QuestionPage Search(QuestionSearch search) => _questionsOperations.Search(search);

        public List<QuestionsDataModel> list()
        {
            var questions = _questionsOperations.list();
            return (questions);
        }
    }
}
