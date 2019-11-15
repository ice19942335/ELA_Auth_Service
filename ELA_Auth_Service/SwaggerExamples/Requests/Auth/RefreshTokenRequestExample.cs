using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Requests.Auth
{
    public class RefreshTokenRequestExample : IExamplesProvider<RefreshTokenRequest>
    {
        public RefreshTokenRequest GetExamples()
        {
            return new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIyZDUwMDhmNy0yZDU4LTRmMmUtODViMC0zZjI3ZjM4NWE5ODEiLCJ1c2VySWQiOiJkZTNhNWQ0Yi04NTQzLTRkMmUtOGE5Zi0zOWM0Zjk3OTZiYWQiLCJlbWFpbCI6InNhbS5hdGtpbnNAZ21haWwuY29tIiwicm9sZSI6IlVzZXIiLCJuYmYiOjE1NzM3MzYzMDYsImV4cCI6MTU3MzczNzIwNSwiaWF0IjoxNTczNzM2MzA2fQ.WhK8nJ8wnhMRctBbVM0ixt4vmt59iv57mtq-kwOzK-s",
                RefreshToken = "c573c6aa-02be-4fd4-87a6-e955427b2e6e"
            };
        }
    }
}
