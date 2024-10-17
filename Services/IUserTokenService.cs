using SampleDotNet.Models;

namespace SampleDotNet.Services
{
    public interface IUserTokenService
    {
        public void AddToken(string token, string userId);
        public void RemoveToken(string token);
        public bool IsBlackListed(string userId);
        public void MarkAllTokenAsBlackListedByUser(string userId);
        public void RemoveAllTokenByUser(string userid);
    }
}
