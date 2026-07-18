using AuraCommerce.IdentityApi.Core.DTOs;
using AuraCommerce.IdentityApi.Core.Entities;

namespace AuraCommerce.IdentityApi.Core.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResult> Register(RegisterDto registerDto);
        public Task<AuthResult> Login(LoginDto loginDto);
    }
}
