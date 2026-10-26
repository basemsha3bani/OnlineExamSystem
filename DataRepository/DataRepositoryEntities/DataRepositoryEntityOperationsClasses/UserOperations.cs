using Domain.Entities;
using Domain.Aggregates;
using AutoMapper;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
using DataRepository.GateWay;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class UserOperations : IUserOperations
    {
        private readonly ContextGateway<User> gateway;
        private readonly IMapper mapper;
        public UserOperations(ContextGateway<User> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public void Add(LoginDTO model)
        {
            var user = mapper.Map<User>(model);
            user.ChangeId(0);
            gateway.Add(user); gateway.SaveChanges();
        }
        public LoginDTO Validate(LoginDTO model)
        {
            if (model == null) return null;
            return mapper.Map<LoginDTO>(gateway.GetById(u => u.Username == model.Username && u.PasswordHash == model.Password));
        }
    }
}
