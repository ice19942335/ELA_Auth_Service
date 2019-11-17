using ELA_Auth_Service.Contracts.V1.Responses.Identity.Email;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Responses.Email
{
    public class EmailConfirmationResponseExample : IExamplesProvider<ConfirmationEmailResponse>
    {
        public ConfirmationEmailResponse GetExamples()
        {
            return new ConfirmationEmailResponse
            {
                Errors = new []{ "User does not exist anymore" },
                CriticalError = true
            };
        }
    }
}
