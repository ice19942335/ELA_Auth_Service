using ELA_Auth_Service.Contracts.V1.Requests.Identity.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Requests.Auth
{
    public class UserRegistrationRequestExample : IExamplesProvider<UserRegistrationRequest>
    {
        public UserRegistrationRequest GetExamples()
        {
            return new UserRegistrationRequest
            {
                Name = "Sam",
                Email = "sam.atkins@gmail.com",
                Password = "Password123!"
            };
        }
    }
}
