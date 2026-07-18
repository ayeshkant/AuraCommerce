using Microsoft.AspNetCore.Identity;

namespace AuraCommerce.IdentityApi.Core.Entities
{
    public class AuthResult
    {
        public bool isSucceeded { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
        public IdentityUser? AuthUser { get; set; }
        public bool validPassword { get; set; }
        public IList<string> Roles { get; set; }
    }
}
