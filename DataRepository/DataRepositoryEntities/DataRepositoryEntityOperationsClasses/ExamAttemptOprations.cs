using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class ExamAttemptOprations : IExamAttemptOprations
    {

        private readonly ContextGateway<ExaminerAttempt> gateway;
        private readonly IMapper mapper;
        public ExamAttemptOprations(ContextGateway<ExaminerAttempt> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
       
       
        public int Add(ExamAttemptDataModel model) 
        {
            ExaminerAttempt entity = mapper.Map<ExaminerAttempt>(model);

            gateway.Add(entity);
            gateway.SaveChanges();
            return entity.Id;
        }
        public void Edit(ExamAttemptDataModel model)
        {
            var entity = gateway.GetById(e => e.Id == model.Id) ?? throw new InvalidOperationException("Difficulty level not found.");
            mapper.Map(model, entity); gateway.SaveChanges();
        }
        public void Delete(int id)
        {
            var entity = gateway.GetById(e => e.Id == id);
            if (entity == null) return;
            gateway.Delete(entity); gateway.SaveChanges();
        }
        public ExamAttemptDataModel GetById(int id) => mapper.Map<ExamAttemptDataModel>(gateway.GetById(e => e.Id == id));
        public List<ExamAttemptDataModel> listByUser(int userId) => mapper.Map<List<ExamAttemptDataModel>>(gateway.List(l=>l.UserId==userId));

        public ExamAttemptDataModel CheckIfAttemptSubmitted(int id)
        {
            ExaminerAttempt attempt = gateway.GetById(e => e.Id == id);
            return attempt.Status == "Submitted" ? mapper.Map<ExamAttemptDataModel>(attempt) : null;
        }

        

        public List<ExamAttemptDataModel> listSubmitted()
        {
           return mapper.Map<List<ExamAttemptDataModel>>(gateway.List(l=> l.Status == "Submitted"));
        }
    }
}
