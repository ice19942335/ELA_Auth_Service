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
            Assert.Equal(false, authFailedResponse?.CriticalError);
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
                    CriticalError = true,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Register(new UserRegistrationRequest());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);
            
            Assert.Equal(400, statusCode);
            Assert.Equal(false, authFailedResponse?.CriticalError);
            Assert.Equal(authFailedResponse.Errors.ToArray()[0], "User with this email address already exists");
        }
    }
}
