using AutoMapper;
using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class UserOperations : IUserOperations
    {
        private readonly ContextGateway<User> gateway;
        private readonly IMapper mapper;
        public UserOperations(ContextGateway<User> gateway, IMapper mapper)
        { this.gateway = gateway; this.mapper = mapper; }
        public void Add(LoginModel model)
        {
            var user = mapper.Map<User>(model);
            user.Id = 0;
            gateway.Add(user); gateway.SaveChanges();
        }
        public LoginModel Validate(LoginModel model)
        {
            if (model == null) return null;
            return mapper.Map<LoginModel>(gateway.GetById(u => u.Username == model.Username && u.PasswordHash == model.Password));
        }
    }
}
