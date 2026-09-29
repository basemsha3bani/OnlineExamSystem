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
            public ExamSectionsDataModel    Map(ExamSections repository)
            {
                return new ExamSectionsDataModel
                {
                    ExamId = repository.ExamId,
                    Id = repository.Id,
                    Percentage = repository.Percentage,
                    SectionName = repository.SectionName,
                    ExamSectionRulesDataModel=repository.examSectionRules?.Select(s=>new ExamSectionRulesDataModel
                    {
                        Id=s.Id,
                        SectionId=s.SectionId,
                        DifficultyLevelId=s.DifficultyLevelId,
                        NoOfQuestions   =s.NoOfQuestions,
                    }).ToList(),
                    exam=new ExamDataModel
                    {
                        Id=repository.Exam.Id,
                        StudySubjectId=repository.Exam.StudySubjectId,  
                      
                    }
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
                List<ExamSectionsDataModel> examSections=                ContextGateway<ExamSections>.List(l => l.ExamId == examId,l=>l.examSectionRules,l2=>l2.Exam).Select(s=>this.Map(s)).ToList();
                return examSections;

            }

        public ExamSectionsDataModel GeById(int id)
        {
            var examSection = ContextGateway<ExamSections>.GetById(g => g.Id == id, g => g.examSectionRules);
            
           return this.Map(examSection);
           
        }
    }
   


}
