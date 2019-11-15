using ELA_Auth_Service.Contracts.V1.Responses.Authentication.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Responses.Auth
{
    public class AuthFailedResponseExample : IExamplesProvider<AuthFailedResponse>
    {
        public AuthFailedResponse GetExamples()
        {
            return new AuthFailedResponse
            {
                Errors = new[] { "Error message, show to user if not critical" },
                CriticalError = false
            };
        }
    }
}
