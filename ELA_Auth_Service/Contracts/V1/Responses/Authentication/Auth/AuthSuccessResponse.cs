namespace ELA_Auth_Service.Contracts.V1.Responses.Authentication.Auth
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
