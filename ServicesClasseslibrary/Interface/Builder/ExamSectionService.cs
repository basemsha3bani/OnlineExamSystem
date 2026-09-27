using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public class ExamSectionService : IExamSectionService
    {
        private readonly IExamSectionsOperations _examSectionsOperations;
        public ExamSectionService(IExamSectionsOperations examSectionsOperations)
        {
            _examSectionsOperations = examSectionsOperations;
        }

        public List<ExamSectionsDataModel> List(int examId) => _examSectionsOperations.List(examId);
        public void Add(ExamSectionsDataModel m) => _examSectionsOperations.Add(new ExamSectionsDataModel { ExamId = m.ExamId, SectionName = m.SectionName, Percentage = m.Percentage });
    }
}

