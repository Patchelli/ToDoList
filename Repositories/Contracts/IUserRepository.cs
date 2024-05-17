using ToDo.API.Models;

namespace ToDo.API.Repositories.Contracts
{
    public interface IUserRepository
    {
        void AddUser(ApplicationUser user, string password);

        ApplicationUser GetUser(string email, string password);

        ApplicationUser GetUserById(string id);

    }
}
