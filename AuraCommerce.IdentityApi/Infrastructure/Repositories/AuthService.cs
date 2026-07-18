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
            authResult.AuthUser = await _userManager.FindByEmailAsync(loginDto.Email);
            if (authResult.AuthUser != null)
            {
                authResult.validPassword = await _userManager.CheckPasswordAsync(authResult.AuthUser, loginDto.Password);
                if (authResult.validPassword)
                {
                    authResult.Roles = await _userManager.GetRolesAsync(authResult.AuthUser);
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
                authResult.isSucceeded = isInRole.Succeeded;
                authResult.Errors = isInRole.Errors;
                return authResult;
            }
            authResult.isSucceeded = result.Succeeded;
            authResult.Errors = result.Errors;
            return authResult;
        }
    }
}
