using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1;
using ELA_Auth_Service.Contracts.V1.Requests.Identity.Auth;
using ELA_Auth_Service.Contracts.V1.Responses.Identity.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using Assert = Xunit.Assert;

namespace ELA_Auth_Service.IntegrationTests.Controllers.V1
{
    [TestClass]
    public  class AuthenticationController : IntegrationTest
    {
        [TestMethod]
        public async Task Register_WithoutAnyPosts_ReturnsErrorsArray_Critical_True()
        {
            //Arrange
            await AuthenticateAdminAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Authentication.Register, new UserRegistrationRequest());
            var failedLoginResult = await response.Content.ReadAsAsync<AuthFailedResponse>();

            //Assert
            Assert.Contains("Please make sure you send correct request, field can't be null", failedLoginResult.Errors);
        }
    }
}
