using System.ComponentModel.DataAnnotations;

namespace ELA_Auth_Service.Contracts.V1.Requests.Identity.Password
{
    public class PasswordResetRequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
