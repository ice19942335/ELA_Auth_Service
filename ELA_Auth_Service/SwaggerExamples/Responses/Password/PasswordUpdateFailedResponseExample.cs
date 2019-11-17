using ELA_Auth_Service.Contracts.V1.Responses.Identity.Password;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Responses.Password
{
    public class PasswordUpdateFailedResponseExample : IExamplesProvider<PasswordUpdateFailedResponse>
    {
        public PasswordUpdateFailedResponse GetExamples()
        {
            return new PasswordUpdateFailedResponse
            {
                Errors = new []{ "This user does not exist anymore" },
                CriticalError = true
            };
        }
    }
}
