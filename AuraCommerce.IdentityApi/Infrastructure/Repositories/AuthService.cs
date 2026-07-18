using System.Linq;
using AuraCommerce.IdentityApi.Core.DTOs;
using AuraCommerce.IdentityApi.Core.Entities;
using AuraCommerce.IdentityApi.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuraCommerce.IdentityApi.Infrastructure.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        public AuthService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<AuthResult> Login(LoginDto loginDto)
        {
            var authResult = new AuthResult();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user != null)
            {
                authResult.UserId = user.Id;
                authResult.UserName = user.UserName;
                authResult.ValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (authResult.ValidPassword)
                {
                    authResult.Roles = await _userManager.GetRolesAsync(user);
                }
            }
            return authResult;
        }

        public async Task<AuthResult> Register(RegisterDto registerDto)
        {
            var authResult = new AuthResult();
            IdentityUser identityUser = new IdentityUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email
            };
            var result = await _userManager.CreateAsync(identityUser, registerDto.Password);
            if (result.Succeeded)
            {
                var isInRole = await _userManager.AddToRoleAsync(identityUser, "Customer");
                authResult.IsSucceeded = isInRole.Succeeded;
                authResult.Errors = isInRole.Errors.Select(e => e.Description);
                return authResult;
            }
            authResult.IsSucceeded = result.Succeeded;
            authResult.Errors = result.Errors.Select(e => e.Description);
            return authResult;
        }
    }
}
