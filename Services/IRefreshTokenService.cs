using SampleDotNet.Models;

namespace SampleDotNet.Services
{
    public interface IRefreshTokenService
    {
        public void AddRefreshToken(CreateRefreshTokenDto createRefreshToken);
        public void RemoveRefreshToken(string Jti);
        public bool IsValidToken(string Jti);
    }
}
