using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using OnlineExamSystem.Application.Interface.DataRepository;

namespace OnlineExamSystem.Application.Interface
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