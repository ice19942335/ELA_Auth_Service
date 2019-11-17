using ELA_Auth_Service.Contracts.V1.Requests.Identity.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Requests.Auth
{
    public class UserLoginRequestExample : IExamplesProvider<UserLoginRequest>
    {
        public UserLoginRequest GetExamples()
        {
            return new UserLoginRequest
            {
                Email = "sam.atkins@gmail.com",
                Password = "Password123!"
            };
        }
    }
}
