using ELA_Auth_Service.Contracts.V1.Requests.Identity.Password;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Requests.Password
{
    public class PasswordResetRequestExample : IExamplesProvider<PasswordResetRequest>
    {
        public PasswordResetRequest GetExamples()
        {
            return new PasswordResetRequest
            {
                Email = "sam.atkins@gmail.com"
            };
        }
    }
}
