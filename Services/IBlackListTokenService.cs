namespace SampleDotNet.Services
{
    public interface IBlackListTokenService
    {
        public void AddToken(string token);
        public void RemoveToken(string token);
        public bool IsBlackListed(string token);
    }
}
