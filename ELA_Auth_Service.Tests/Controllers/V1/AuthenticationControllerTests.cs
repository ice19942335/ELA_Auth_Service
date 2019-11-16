using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Auth;
using ELA_Auth_Service.Contracts.V1.Responses.Authentication.Auth;
using ELA_Auth_Service.Controllers.V1;
using ELA_Auth_Service.Data;
using ELA_Auth_Service.Data._MySqlDataContext;
using ELA_Auth_Service.Domain.DTO;
using ELA_Auth_Service.Domain.Entities;
using ELA_Auth_Service.Options;
using ELA_Auth_Service.Services.Implementation;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renci.SshNet;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Xunit.Sdk;

using Assert = Xunit.Assert;

namespace ELA_Auth_Service.Tests.Controllers.V1
{
    [TestClass]
    public class AuthenticationControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            
        }

        #region Register

        [TestMethod]
        public void Register_Method_Returns_BadRequest_On_IncorrectRequest()
        {
            var authServiceMock = new Mock<IAuthenticationService>();

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Register(new UserRegistrationRequest());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Please make sure you send correct request, field can't be null", authFailedResponse.Errors);
        }

        [TestMethod]
        public void Register_Method_Returns_BadRequest_On_ModelStateFalse()
        {
            var authServiceMock = new Mock<IAuthenticationService>();

            var controller = new AuthenticationController(authServiceMock.Object);

            controller.ModelState.AddModelError("", "Model is incorrect");

            var result = controller.Register(new UserRegistrationRequest
            {
                Name = "TestName",
                Email = "WrongEmail",
                Password = "WrongPassword"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            var modelStateIsValid = authFailedResponse.Errors?.Count() > 0;

            Assert.Equal(400, statusCode);
            Assert.False(authFailedResponse.CriticalError);
            Assert.True(modelStateIsValid);
        }

        [TestMethod]
        public void Register_Method_Returns_BadRequest_On_ExistingUserInSystem()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "User with this email address already exists" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Register(new UserRegistrationRequest
            {
                Name = "TestName",
                Email = "testEmail@gmail.com",
                Password = "TestPassword123!"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);
            
            Assert.Equal(400, statusCode);
            Assert.False(authFailedResponse.CriticalError);
            Assert.Contains("User with this email address already exists", authFailedResponse.Errors);
        }

        [TestMethod]
        public void Register_Method_Returns_BadRequest_On_UserCreation()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "Passwords must have at least one non alphanumeric character." },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Register(new UserRegistrationRequest
            {
                Name = "TestName",
                Email = "testEmail@gmail.com",
                Password = "TestPassword123"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Passwords must have at least one non alphanumeric character.", authFailedResponse.Errors);
        }

        [TestMethod]
        public void Register_Method_Returns_BadRequest_On_EntryWritingToDB()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "Problem on writing entry in MySqlDB" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Register(new UserRegistrationRequest
            {
                Name = "TestName",
                Email = "testEmail@gmail.com",
                Password = "TestPassword123!"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Problem on writing entry in MySqlDB", authFailedResponse.Errors);
            Assert.True(authFailedResponse.CriticalError);
        }

        [TestMethod]
        public void Register_Method_Returns_OK_On_Success_Registration()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                    RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2",
                    Success = true
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Register(new UserRegistrationRequest
            {
                Name = "TestName",
                Email = "testEmail@gmail.com",
                Password = "TestPassword123!"
            });

            var badRequestResult = Assert.IsType<OkObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthSuccessResponse>(badRequestResult.Value);

            Assert.Equal(200, statusCode);
            Assert.NotNull(authResponse.Token);
            Assert.NotNull(authResponse.RefreshToken);
        }

        #endregion
    }
}
