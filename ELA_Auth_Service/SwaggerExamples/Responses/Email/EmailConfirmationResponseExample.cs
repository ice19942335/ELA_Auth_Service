using ELA_Auth_Service.Contracts.V1.Responses.Identity.Email;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Responses.Email
{
    public class EmailConfirmationResponseExample : IExamplesProvider<EmailConfirmationResponse>
    {
        public EmailConfirmationResponse GetExamples()
        {
            return new EmailConfirmationResponse
            {
                Errors = new []{ "User does not exist anymore" },
                CriticalError = true
            };
        }
    }
}
