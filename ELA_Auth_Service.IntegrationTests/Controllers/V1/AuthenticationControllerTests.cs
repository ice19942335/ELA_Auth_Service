using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ELA_Auth_Service._Data._MySqlDataContext;
using ELA_Auth_Service.Contracts.V1;
using ELA_Auth_Service.Contracts.V1.Requests.Identity.Auth;
using ELA_Auth_Service.Contracts.V1.Responses.Identity.Auth;
using ELA_Auth_Service.IdentityInitializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using Assert = Xunit.Assert;

namespace ELA_Auth_Service.IntegrationTests.Controllers.V1
{
    [TestClass]
    public  class AuthenticationControllerTests : IntegrationTest 
    {
        [TestMethod]
        public async Task Register_WithoutAnyPosts_ReturnsErrorsArray_Critical_True()
        {
            //Arrange
            var expectedError = "Please make sure you send correct request, field can't be null";

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Authentication.Register, new UserRegistrationRequest());
            var failedLoginResult = await response.Content.ReadAsAsync<AuthFailedResponse>();

            //Assert
            Assert.Contains(expectedError, failedLoginResult.Errors);
            Assert.True(failedLoginResult.CriticalError);
        }

        [TestMethod]
        public async Task Register_ExistingUser_ReturnsErrorsArray_Critical_False()
        {
            //Arrange
            var expectedError = "User with this email address already exists";

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Authentication.Register, new UserRegistrationRequest
            {
                Name = DefaultIdentity.DefaultAdminUserName,
                Email = DefaultIdentity.DefaultAdminUserName,
                Password = DefaultIdentity.DefaultAdminPassword
            });
            var failedLoginResult = await response.Content.ReadAsAsync<AuthFailedResponse>();

            //Assert
            Assert.Contains(expectedError, failedLoginResult.Errors);
            Assert.False(failedLoginResult.CriticalError);
        }

        [TestMethod]
        public async Task Register_WrongPassword_ReturnsErrorsArray_Critical_False()
        {
            //Arrange
            var expectedErrorsList = new[]
            {
                "Passwords must be at least 6 characters.",
                "Passwords must have at least one non alphanumeric character.",
                "Passwords must have at least one digit ('0'-'9').",
                "Passwords must have at least one lowercase ('a'-'z').",
                "Passwords must have at least one uppercase ('A'-'Z').",
                "Passwords must use at least 1 different characters."
            };

            //Act
            var response = await TestClient.PostAsJsonAsync(
                ApiRoutes.Authentication.Register,
                new UserRegistrationRequest
                {
                    Name = "Sam",
                    Email = $"Sam_{Guid.NewGuid().ToString().Substring(0, 6)}@mail.com",
                    Password = ""
                });

            var failedLoginResult = await response.Content.ReadAsAsync<AuthFailedResponse>();

            //Assert
            failedLoginResult.Errors.ToList().ForEach(x => Assert.Contains(x, failedLoginResult.Errors));
            Assert.False(failedLoginResult.CriticalError);
        }


        [TestMethod]
        public async Task Register_Success_ReturnsTokens_Critical_False()
        {
            //Arrange

            //Act
            var response = await TestClient.PostAsJsonAsync(
                ApiRoutes.Authentication.Register,
                new UserRegistrationRequest
                {
                    Name = "Sam",
                    Email = $"Sam_{Guid.NewGuid().ToString().Substring(0, 6)}@mail.com",
                    Password = "Password123!"
                });

            var failedLoginResult = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            //Assert
            Assert.NotNull(failedLoginResult.Token);
            Assert.NotNull(failedLoginResult.RefreshToken);
        }
    }
}
