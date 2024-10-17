using System.IdentityModel.Tokens.Jwt;

namespace SampleDotNet.Models
{
    public class GenerateAccessTokenResponse
    {
        public JwtSecurityToken Token { get; set; }
        public string Jti {  get; set; }
    }
}
