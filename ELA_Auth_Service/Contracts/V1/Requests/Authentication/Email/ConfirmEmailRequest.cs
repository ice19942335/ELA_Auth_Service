namespace ELA_Auth_Service.Contracts.V1.Requests.Authentication.Email
{
    public class ConfirmEmailRequest
    {
        public string UserId { get; set; }

        public string Code { get; set; }
    }
}
