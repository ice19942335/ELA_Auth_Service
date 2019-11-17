namespace ELA_Auth_Service.Contracts.V1.Requests.Identity.Email
{
    public class ConfirmEmailRequest
    {
        public string UserId { get; set; }

        public string Token { get; set; }
    }
}
