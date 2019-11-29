namespace ELA_Auth_Service.Contracts.V1.Responses.Identity.Auth
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
