using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Email;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Requests.Email
{
    public class EmailConfirmationRequestExample : IExamplesProvider<EmailConfirmationRequest>
    {
        public EmailConfirmationRequest GetExamples()
        {
            return new EmailConfirmationRequest
            {
                Email = "sam.atkins@gmail.com"
            };
        }
    }
}
