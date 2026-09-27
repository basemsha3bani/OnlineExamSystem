using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using DataRepository.ModelMapper.Interface;
using System.Collections.Generic;
using System.Linq;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
 
    

        public class ExamSectionsOperations: IExamSectionsOperations, IModelMapper<ExamSectionsDataModel,ExamSections>
        {
            public ExamSectionsDataModel Map(ExamSections repository)
            {
                return new ExamSectionsDataModel
                {
                    ExamId = repository.ExamId,
                    Id = repository.Id,
                    Percentage = repository.Percentage,
                    SectionName = repository.SectionName
                };
            }

            public void Add(ExamSectionsDataModel examSectionsDataModel)
            {
                ExamSections examSections = new ExamSections();
                examSections.Id = examSectionsDataModel.Id;
                examSections.ExamId = examSections.ExamId;

                examSections.SectionName = examSections.SectionName;
                examSections.Percentage = examSectionsDataModel.Percentage;




                ContextGateway<ExamSections>.Add(examSections);
            }

            public List<ExamSectionsDataModel> List(int examId)
            {
                List<ExamSectionsDataModel> examSections=                ContextGateway<ExamSections>.List(l => l.ExamId == examId).Select(s=>this.Map(s)).ToList();
                return examSections;

            }
        }
    
    
}
