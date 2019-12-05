using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ELA_Auth_Service._Data.ElaAuthDB;
using ELA_Auth_Service._Data.ElaDataDB;
using ELA_Auth_Service.Contracts.V1;
using ELA_Auth_Service.Contracts.V1.Requests.Identity.Auth;
using ELA_Auth_Service.Contracts.V1.Responses.Identity.Auth;
using ELA_Auth_Service.IdentityInitializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ELA_Auth_Service.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAdminAsync()
        { 
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetAdminJwtAsync());
        }

        private async Task<string> GetAdminJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Authentication.Login, new UserLoginRequest
            {
                Email = DefaultIdentity.DefaultAdminName,
                Password = DefaultIdentity.DefaultAdminPassword
            });

            var loginResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return loginResponse.Token;
        }
    }
}
