using System.ComponentModel.DataAnnotations;
using SendGrid.Helpers.Mail;

namespace ELA_Auth_Service.Contracts.V1.Requests.Authentication.Password
{
    public class PasswordResetRequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
