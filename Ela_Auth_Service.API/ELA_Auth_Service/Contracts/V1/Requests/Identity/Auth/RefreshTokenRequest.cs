namespace ELA_Auth_Service.Contracts.V1.Requests.Identity.Auth
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
