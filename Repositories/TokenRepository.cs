using System;
using System.Collections.Generic;
using System.Linq;
using ToDo.API.Database;
using ToDo.API.Models;
using ToDo.API.Repositories.Contracts;

namespace ToDo.API.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TaskContext _taskContext;

        public TokenRepository(TaskContext taskContext)
        {
            _taskContext = taskContext;
        }

        public Token? GetToken(string refreshToken)
        {
            return _taskContext.Token.Where(t => t.RefreshToken == refreshToken && t.IsExpired == false).FirstOrDefault();
        }


        public void AddToken(Token token)
        {
            _taskContext.Token.Add(token);
            _taskContext.SaveChanges();
        }

        public void UpdateToken(Token token)
        {
            _taskContext.Token.Update(token);
            _taskContext.SaveChanges();
        }
    }
}
