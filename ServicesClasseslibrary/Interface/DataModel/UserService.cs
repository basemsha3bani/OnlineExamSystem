using DataModel;
using DataRepository.DataRepositoryEntities;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;

namespace ServicesClasseslibrary.Interface
{
    public class UserService:IUserService
    {
       private readonly IUserOperations _userOperations;
        public UserService(IUserOperations userOperations)
        {
            _userOperations= userOperations;
        }

        public void Add(LoginModel user)
        {
            _userOperations.Add(user);
        }

        public  LoginModel Validate(LoginModel model)
        {
            return _userOperations.Validate(model);
        }
    }
}