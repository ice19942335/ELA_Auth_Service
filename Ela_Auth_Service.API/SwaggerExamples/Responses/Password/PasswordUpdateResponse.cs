using ELA_Auth_Service.Contracts.V1.Responses.Identity.Password;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Responses.Password
{
    public class PasswordUpdateResponseExample : IExamplesProvider<PasswordUpdateResponse>
    {
        public PasswordUpdateResponse GetExamples()
        {
            return new PasswordUpdateResponse
            {
                Errors = new []{ "This user does not exist anymore" },
                CriticalError = true
            };
        }
    }
}
