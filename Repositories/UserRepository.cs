using Microsoft.AspNetCore.Identity;
using System.Text;
using ToDo.API.Models;
using ToDo.API.Repositories.Contracts;

namespace ToDo.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public void AddUser(ApplicationUser user, string password)
        {
            var result = _userManager.CreateAsync(user, password).Result;
            if (!result.Succeeded)
            {
                StringBuilder errors = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errors.Append(error.Description);
                }
                throw new Exception($" Usuario não cadastrado{errors.ToString()}");
            }
        }

        public ApplicationUser GetUser(string email, string password)
        {
            var user = _userManager.FindByEmailAsync(email).Result;

            if (!(user is null) && _userManager.CheckPasswordAsync(user, password).Result)
                return user;
            else
                throw new Exception("Usuario não encontrado");
        }
    }
}
