namespace ELA_Auth_Service.Contracts.V1.Requests.Identity.Password
{
    public class PasswordUpdateRequest
    {
        public string UserId { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }
}
