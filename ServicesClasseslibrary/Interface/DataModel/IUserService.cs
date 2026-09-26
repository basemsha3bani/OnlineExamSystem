using DataModel;
using DataRepository.DataRepositoryEntities;

namespace ServicesClasseslibrary.Interface
{
    public interface IUserService
    {
        void Add(LoginModel user);
        LoginModel Validate(LoginModel model);
    }
}
