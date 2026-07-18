using AuraCommerce.IdentityApi.Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            IdentityUser identityUser = new IdentityUser
            {
                Email = registerDto.Email,
                UserName=registerDto.Email
            };
            var result=await _userManager.CreateAsync(identityUser, registerDto.Password);
            if (result.Succeeded)
            {
                var isInRole = await _userManager.AddToRoleAsync(identityUser, "Customer");
                if (isInRole.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(isInRole.Errors);
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto!=null)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null)
                {
                    var validPassword=await _userManager.CheckPasswordAsync(user, loginDto.Password);
                    if (validPassword)
                    {
                        var roles= await _userManager.GetRolesAsync(user);
                        var token=GenerateToken(user.Id,user.UserName,roles.ToList());
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
