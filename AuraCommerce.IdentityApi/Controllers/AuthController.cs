using AuraCommerce.IdentityApi.Core.DTOs;
using AuraCommerce.IdentityApi.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuraCommerce.IdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result=await _authService.Register(registerDto);
            if (result.IsSucceeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto!=null)
            {
                var user = await _authService.Login(loginDto);
                if (user.UserId != null)
                {
                    if (user.ValidPassword)
                    {
                        var token = GenerateToken(user.UserId, user.UserName, user.Roles.ToList());
                        return Ok(new { Token = token });
                    }
                    return Unauthorized($"Password provide for user {user.UserName} is invalid");
                }
                return Unauthorized($"User {loginDto.Email} does not exist");
            }
            return BadRequest();
        }
        private string GenerateToken(string userId, string userName, List<string> roles)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName)
            };
            foreach (var role in roles)
            {
                var claim=new Claim(ClaimTypes.Role, role);
                claims.Add(claim);
            }
            var securityTokenObject = new JwtSecurityToken(issuer, audience, claims, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), credentials);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityTokenObject);

            return tokenString;
        }
    }
}
