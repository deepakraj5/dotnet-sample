using SampleDotNet.Data;
using SampleDotNet.Models.Entities;

namespace SampleDotNet.Services
{
    public class BlackListTokenService : IBlackListTokenService
    {
        private readonly ApplicationDBContext _dbContext;

        public BlackListTokenService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddToken(string token)
        {
            var blackList = new BlackListToken()
            {
                token = token
            };

            _dbContext.BlackListTokens.Add(blackList);
            _dbContext.SaveChanges();
        }

        public bool IsBlackListed(string token)
        {
            var blackList = _dbContext.BlackListTokens.Find(token);
            if(blackList is null)
            {
                return false;
            }
            return true;
        }

        public void RemoveToken(string token)
        {
            var blackList = _dbContext.BlackListTokens.Find(token);
            if (blackList is null)
            {
                throw new KeyNotFoundException("Given token not found");
            }

            _dbContext.BlackListTokens.Remove(blackList);
            _dbContext.SaveChanges();
        }
    }
}
