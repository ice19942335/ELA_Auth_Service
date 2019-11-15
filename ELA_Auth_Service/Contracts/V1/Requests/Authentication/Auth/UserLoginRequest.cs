using System.ComponentModel.DataAnnotations;

namespace ELA_Auth_Service.Contracts.V1.Requests.Authentication.Auth
{
    public class UserLoginRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
