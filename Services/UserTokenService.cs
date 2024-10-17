using SampleDotNet.Data;
using SampleDotNet.Models;
using SampleDotNet.Models.Entities;

namespace SampleDotNet.Services
{
    public class UserTokenService : IUserTokenService
    {
        private readonly ApplicationDBContext _dbContext;

        public UserTokenService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddToken(string token, string userId)
        {
            var userToken = new UserAccToken()
            {
                Token = token,
                UserId = userId,
                IsBlackListed = false
            };

            _dbContext.UserAccTokens.Add(userToken);
            _dbContext.SaveChanges();
        }

        public bool IsBlackListed(string userId)
        {
            var userTokens = _dbContext.UserAccTokens.Where(token => token.UserId == userId).ToList();
            // implement to check IsBlackListed
            if (userTokens.Count == 0 || userTokens is null)
            {
                return true;
            }
            return false;
        }

        public void RemoveToken(string token)
        {
            var userToken = _dbContext.UserAccTokens.Find(token);
            if (userToken is null)
            {
                throw new KeyNotFoundException("Given token not found");
            }

            _dbContext.UserAccTokens.Remove(userToken);
            _dbContext.SaveChanges();
        }

        public void MarkAllTokenAsBlackListedByUser(string userId)
        {
            var userTokens = _dbContext.UserAccTokens.Where(token => token.UserId == userId).ToList();
            if (userTokens is null || userTokens.Count == 0)
            {
                return;
            }

            foreach(var userToken in userTokens)
            {
                userToken.IsBlackListed = true;
            }

            _dbContext.SaveChanges();
        }

        public void RemoveAllTokenByUser(string userId)
        {
            var userTokens = _dbContext.UserAccTokens.Where(token => token.UserId == userId).ToList();
            if (userTokens is null || userTokens.Count == 0)
            {
                return;
            }

            _dbContext.UserAccTokens.RemoveRange(userTokens);
            _dbContext.SaveChanges();
        }
    }
}
