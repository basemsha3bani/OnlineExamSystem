using Domain.DTOs;

namespace OnlineExamSystem.Application.Interface.DataRepository
{
    public interface IUserOperations
    {
        void Add(LoginDTO loginModel);
        LoginDTO Validate(LoginDTO loginModel);
    }
}
