using SampleDotNet.Data;
using SampleDotNet.Models;
using SampleDotNet.Models.Entities;

namespace SampleDotNet.Services
{
    public class RefreshTokenService: IRefreshTokenService
    {
        private readonly ApplicationDBContext _dbContext;

        public RefreshTokenService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRefreshToken(CreateRefreshTokenDto createRefreshToken)
        {
            var refreshToken = new RefreshToken
            {
                Jti = createRefreshToken.Jti,
                ExpiresIn = createRefreshToken.ExpiresIn
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            _dbContext.SaveChanges();
        }

        public void RemoveRefreshToken(string Jti)
        {
            var refreshToken = _dbContext.RefreshTokens.Find(Jti);
            if(refreshToken is null)
            {
                throw new KeyNotFoundException("Given Jti not found");
            }

            _dbContext.RefreshTokens.Remove(refreshToken);
            _dbContext.SaveChanges();
        }

        public bool IsValidToken(string Jti)
        {
            var refreshToken = _dbContext.RefreshTokens.Find(Jti);

            if (refreshToken is null)
            {
                throw new KeyNotFoundException("Given Jti not found");
            }

            int expirationResult = DateTime.Compare(refreshToken.ExpiresIn, DateTime.Now);

            if(expirationResult < 0)
            {
                return false;
            }

            return true;
        }
    }
}
