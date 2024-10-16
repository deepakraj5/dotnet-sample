using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SampleDotNet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleDotNet.Services
{
    public class JwtService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _configuration;

        public JwtService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, 
                        IConfiguration configuration, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<JwtSecurityToken> CreateAccessToken(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                claims: authClaims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                                                                SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        public JwtSecurityToken CreateRefreshToken(IdentityUser user)
        {
            string Jti = Guid.NewGuid().ToString();
            DateTime expiresIn = DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:RefreshTokenExpiryMinutes"]!));

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Jti)
            };

            var refreshToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                expires: expiresIn,
                claims: authClaims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                                                                SecurityAlgorithms.HmacSha256)
            );

            CreateRefreshTokenDto refreshTokenDto = new CreateRefreshTokenDto()
            {
                Jti = Jti,
                ExpiresIn = expiresIn
            };

            _refreshTokenService.AddRefreshToken(refreshTokenDto);

            return refreshToken;
        }
    }
}
