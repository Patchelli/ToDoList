using ToDo.API.Models;

namespace ToDo.API.Repositories.Contracts
{
    public interface ITokenRepository
    {
        void AddToken(Token token);
        Token? GetToken(string refreshToken);
        void UpdateToken(Token token);
    }
}
