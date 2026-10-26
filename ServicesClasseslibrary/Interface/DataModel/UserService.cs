using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;

namespace ServicesClasseslibrary.Interface
{
    public class UserService:IUserService
    {
       private readonly IUserOperations _userOperations;
        public UserService(IUserOperations userOperations)
        {
            _userOperations= userOperations;
        }

        public void Add(LoginDTO user)
        {
            _userOperations.Add(user);
        }

        public  LoginDTO Validate(LoginDTO model)
        {
            return _userOperations.Validate(model);
        }
    }
}