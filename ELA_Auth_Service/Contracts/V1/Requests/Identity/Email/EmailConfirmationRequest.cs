using System.ComponentModel.DataAnnotations;

namespace ELA_Auth_Service.Contracts.V1.Requests.Identity.Email
{
    public class EmailConfirmationRequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
