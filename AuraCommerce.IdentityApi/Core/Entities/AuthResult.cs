using Microsoft.AspNetCore.Identity;

namespace AuraCommerce.IdentityApi.Core.Entities
{
    public class AuthResult
    {
        public bool IsSucceeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string User { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool ValidPassword { get; set; }
        public IList<string> Roles { get; set; }
    }
}
