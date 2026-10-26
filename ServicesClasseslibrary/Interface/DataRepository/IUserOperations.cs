using Domain.DTOs;

namespace ServicesClasseslibrary.Interface.DataRepository
{
    public interface IUserOperations
    {
        void Add(LoginDTO loginModel);
        LoginDTO Validate(LoginDTO loginModel);
    }
}
