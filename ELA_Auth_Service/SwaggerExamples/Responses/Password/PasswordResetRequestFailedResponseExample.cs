using ELA_Auth_Service.Contracts.V1.Responses.Authentication.Password;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Responses.Password
{
    public class PasswordResetRequestFailedResponseExample : IExamplesProvider<PasswordResetRequestFailedResponse>
    {
        public PasswordResetRequestFailedResponse GetExamples()
        {
            return new PasswordResetRequestFailedResponse
            {
                Errors = new[] { "Please make sure you send correct request, field can't be null" },
                CriticalError = true
            };
        }
    }
}
