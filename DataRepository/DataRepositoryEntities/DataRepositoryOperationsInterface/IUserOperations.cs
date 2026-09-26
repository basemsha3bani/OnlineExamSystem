using DataModel;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
    public interface IUserOperations
    {
        void Add(LoginModel loginModel);
        LoginModel Validate(LoginModel loginModel);
    }
}
