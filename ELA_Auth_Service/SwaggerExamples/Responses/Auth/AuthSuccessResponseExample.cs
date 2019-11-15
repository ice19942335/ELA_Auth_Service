using ELA_Auth_Service.Contracts.V1.Responses.Authentication.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Responses.Auth
{
    public class AuthSuccessResponseExample : IExamplesProvider<AuthSuccessResponse>
    {
        public AuthSuccessResponse GetExamples()
        {
            return new AuthSuccessResponse
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJhM2I0ZDc0Zi1kOTk5LTRlOWQtYTRiZi1jZDMxM2ZhZjcwNzAiLCJ1c2VySWQiOiI1MjA5MDFlMC1mZTRlLTRhMDYtOTE5NS05ZjM1YmEwNWUwOTQiLCJlbWFpbCI6ImFsZWtzZWpiaXJ1bGFAZ21haWwuY29tIiwicm9sZSI6WyJBZG1pbiIsIlVzZXIiXSwibmJmIjoxNTczNzI4NTQ4LCJleHAiOjE1NzM3Mjk0NDgsImlhdCI6MTU3MzcyODU0OH0.ayh8vozRp0-mHrReXi7ybRkw-e-zhoCD52Mbr3c8yqw",
                RefreshToken = "c573c6aa-02be-4fd4-87a6-e955427b2e6e"
            };
        }
    }
}
