using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using DataRepository.ModelMapper.Interface;
using System.Linq;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class UserOperations : IUserOperations,IModelMapper<LoginModel,User>
    {
        public void Add(LoginModel loginModel)
        {
            User user = new User { Username = loginModel.Username, PasswordHash = loginModel.Password, Role = loginModel.Role };
            ContextGateway<User>.GetContextInstance();
            ContextGateway<User>.Add(user);
        }

        public LoginModel Map(User user)
        {
            if(user==null)
            {
                return null;
            }
            return new LoginModel
            {
                Id =user.Id,
                Role=user.Role

            };
        }

        public LoginModel Validate(LoginModel loginModel)
        {
           
            if (loginModel == null)
            {
                return null;
            }
            ContextGateway<User>.GetContextInstance();
            User user = ContextGateway<User>.List(l => l.Username == loginModel.Username && l.PasswordHash == loginModel.Password).FirstOrDefault();
            return this.Map(user);


        }
    }
}
