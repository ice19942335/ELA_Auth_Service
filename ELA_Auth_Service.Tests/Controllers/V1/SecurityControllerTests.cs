using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Password;
using ELA_Auth_Service.Contracts.V1.Responses.Authentication.Password;
using ELA_Auth_Service.Controllers.V1;
using ELA_Auth_Service.Domain.DTO;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit.Sdk;

using Assert = Xunit.Assert;

namespace ELA_Auth_Service.Tests.Controllers.V1
{
    [TestClass]
    public class SecurityControllerTests
    {

        #region PasswordResetRequest

        [TestMethod]
        public async Task PasswordResetRequest_Method_Returns_BadRequest_On_IncorrectRequest()
        {
            var securityServiceMock = new Mock<ISecurityService>();

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.PasswordResetRequest(new PasswordResetRequest());

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestObjectResult.StatusCode);
            var passwordResetRequestFailedResponse = Assert.IsAssignableFrom<PasswordResetRequestFailedResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Please make sure you send correct request, field can't be null", passwordResetRequestFailedResponse.Errors);
        }

        [TestMethod]
        public async Task PasswordResetRequest_Method_Returns_BadRequest_On_UserDoNotExistAnymore()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.PasswordResetRequestAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new PasswordUpdateDto
                {
                    Errors = new[] { "User with this email does not exist" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.PasswordResetRequest(new PasswordResetRequest { Email = "sam.atkins@gmail.com" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var passwordResetRequestFailedResponse = Assert.IsAssignableFrom<PasswordResetRequestFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.True(passwordResetRequestFailedResponse.CriticalError);
            Assert.Contains("User with this email does not exist", passwordResetRequestFailedResponse.Errors);
        }

        [TestMethod]
        public async Task PasswordResetRequest_Method_Returns_BadRequest_On_UserWithNotConfirmedEmail()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.PasswordResetRequestAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new PasswordUpdateDto
                {
                    Errors = new[] { "User email is not confirmed, sorry, we can't help you with this" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.PasswordResetRequest(new PasswordResetRequest { Email = "sam.atkins@gmail.com" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var passwordResetRequestFailedResponse = Assert.IsAssignableFrom<PasswordResetRequestFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.False(passwordResetRequestFailedResponse.CriticalError);
            Assert.Contains("User email is not confirmed, sorry, we can't help you with this", passwordResetRequestFailedResponse.Errors);
        }

        [TestMethod]
        public async Task PasswordResetRequest_Method_Returns_BadRequest_On_ProblemWitSendingLinkToTheEmail()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.PasswordResetRequestAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new PasswordUpdateDto
                {
                    Errors = new[] { "Something went wrong, please try again or contact support" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.PasswordResetRequest(new PasswordResetRequest { Email = "sam.atkins@gmail.com" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var passwordResetRequestFailedResponse = Assert.IsAssignableFrom<PasswordResetRequestFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.False(passwordResetRequestFailedResponse.CriticalError);
            Assert.Contains("Something went wrong, please try again or contact support", passwordResetRequestFailedResponse.Errors);
        }

        [TestMethod]
        public async Task PasswordResetRequest_Method_Returns_OK_On_SuccessRequest()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.PasswordResetRequestAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new PasswordUpdateDto { Success = true }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.PasswordResetRequest(new PasswordResetRequest { Email = "sam.atkins@gmail.com" });

            var badRequestResult = Assert.IsType<NoContentResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);

            Assert.Equal(204, statusCode);
        }

        #endregion

    }
}
