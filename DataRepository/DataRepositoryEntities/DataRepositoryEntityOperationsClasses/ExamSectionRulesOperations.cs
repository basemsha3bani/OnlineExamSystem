using DataRepository.GateWay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    namespace DataRepository.DataRepositoryEntities
    {
        public class ExamSectionRulesOperations:IExamSectionRuleOperations
        {
            

            public ExamSectionRulesOperations()
            {
                ContextGateway<ExamSectionRules>.GetContextInstance();
            }

            // Operations talk to Gateway
            public List<ExamSectionRules> GetBySectionId(int sectionId)
            {
                return ContextGateway<ExamSectionRules>
    .List(r => r.SectionId == sectionId,
          r => r.difficultyLevel,
          r => r.section.Exam.StudySubject)
    .ToList();
            }

            public void Add(ExamSectionRules entity)
            {
                ContextGateway<ExamSectionRules>.CreateDatabaseTransaction();
                {
                    try
                    {
                        var existing = ContextGateway<ExamSectionRules>.List(r => r.SectionId == entity.SectionId).ToList();
                            

                        // Your Rules guard: 2 Easy + 3 Hard = 5 total
                        if (existing.Sum(r => r.NoOfQuestions) + entity.NoOfQuestions > 5)
                            throw new Exception("Max 5 questions per section");

                        ContextGateway<ExamSectionRules>.Add(entity);
                        
                        ContextGateway<ExamSectionRules>.Commit();
                    }
                    catch
                    {
                        ContextGateway<ExamSectionRules>.Rollback();
                        throw;                    }
                }
            }
        }
    }


}
