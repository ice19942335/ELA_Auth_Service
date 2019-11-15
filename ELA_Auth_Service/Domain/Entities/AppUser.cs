using Microsoft.AspNetCore.Identity;

namespace ELA_Auth_Service.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
