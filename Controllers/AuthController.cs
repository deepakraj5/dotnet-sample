using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SampleDotNet.Services;
using SampleDotNet.Models;
using Microsoft.AspNetCore.Authorization;

namespace SampleDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserTokenService _userTokenService;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration,
            JwtService jwtService, IRefreshTokenService refreshTokenService,
            IUserTokenService userTokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _userTokenService = userTokenService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] Register register)
        {
            var user = new IdentityUser { UserName = register.Username, Email = register.Email };
            var result = await _userManager.CreateAsync(user, register.Password);

            if(result.Succeeded)
            {
                return Ok(new
                {
                    message = "User created successfully"
                });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            if(!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if(result.Succeeded)
                {
                    return Ok(new { message = "Role added successfully" });
                }

                return BadRequest(result.Errors);
            }

            return BadRequest("Role Already Exists");
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole role)
        {
            var user = await _userManager.FindByNameAsync(role.Username);
            if(user is null)
            {
                return BadRequest("User not exists");
            }

            var result = await _userManager.AddToRoleAsync(user, role.Role);
            if(result.Succeeded)
            {
                _userTokenService.RemoveAllTokenByUser(user.Id);
                return Ok(new { message = "Role assigned successfully" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if(user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var accessToken = await _jwtService.CreateAccessToken(user);
                var refreshToken = _jwtService.CreateRefreshToken(user);

                _userTokenService.AddToken(accessToken.Jti, user.Id);

                return Ok(new { accessToken = new JwtSecurityTokenHandler().WriteToken(accessToken.Token),
                                refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken)});
            }

            return Unauthorized();
        }

        [HttpPost("silent-login")]
        public async Task<IActionResult> SilentLogin([FromBody] RefreshTokenRequest refreshTokenRequest)
        {

            if(refreshTokenRequest.Token is null)
            {
                return Unauthorized();
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(refreshTokenRequest.Token);

            var tokenValidationParams = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),

                // only when expiry time less than 5 min, allow to use seconds
                ClockSkew = TimeSpan.Zero
            };

            handler.ValidateToken(refreshTokenRequest.Token, tokenValidationParams, out var validatedToken);

            var jti = token.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;
            var userName = token.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)!.Value;

            var user = await _userManager.FindByNameAsync(userName);

            if(!_refreshTokenService.IsValidToken(jti) || user is null)
            {
                return Unauthorized("Invalid access token or refresh token");
            }

            var accessToken = await _jwtService.CreateAccessToken(user);
            var refreshToken = _jwtService.CreateRefreshToken(user);

            // remove old refresh token from persistence
            _refreshTokenService.RemoveRefreshToken(jti);

            _userTokenService.AddToken(accessToken.Jti, user.Id);

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(accessToken.Token),
                refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken)
            });
        }
    }
}
