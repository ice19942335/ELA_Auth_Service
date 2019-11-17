using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1.Requests.Identity.Email;
using ELA_Auth_Service.Contracts.V1.Requests.Identity.Password;
using ELA_Auth_Service.Contracts.V1.Responses.Identity.Email;
using ELA_Auth_Service.Contracts.V1.Responses.Identity.Password;
using ELA_Auth_Service.Controllers.V1;
using ELA_Auth_Service.Domain.DTO;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Assert = Xunit.Assert;

namespace ELA_Auth_Service.UnitTests.Controllers.V1
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
            var passwordUpdateResponse = Assert.IsAssignableFrom<PasswordUpdateResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Please make sure you send correct request, field can't be null", passwordUpdateResponse.Errors);
        }

        [TestMethod]
        public void PasswordResetRequest_Method_Returns_BadRequest_On_ModelStateFalse()
        {
            var securityServiceMock = new Mock<ISecurityService>();

            var controller = new SecurityController(securityServiceMock.Object);

            controller.ModelState.AddModelError("", "Model is incorrect");

            var result = controller.PasswordResetRequest(new PasswordResetRequest { Email = "WrongEmail" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<PasswordUpdateResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.False(authFailedResponse.CriticalError);
            Assert.NotNull(authFailedResponse.Errors);
            Assert.NotEmpty(authFailedResponse.Errors);
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
            var passwordUpdateResponse = Assert.IsAssignableFrom<PasswordUpdateResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.True(passwordUpdateResponse.CriticalError);
            Assert.Contains("User with this email does not exist", passwordUpdateResponse.Errors);
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
            var passwordUpdateResponse = Assert.IsAssignableFrom<PasswordUpdateResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.False(passwordUpdateResponse.CriticalError);
            Assert.Contains("User email is not confirmed, sorry, we can't help you with this", passwordUpdateResponse.Errors);
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
            var passwordUpdateResponse = Assert.IsAssignableFrom<PasswordUpdateResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.False(passwordUpdateResponse.CriticalError);
            Assert.Contains("Something went wrong, please try again or contact support", passwordUpdateResponse.Errors);
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

        #region PasswordUpdate

        [TestMethod]
        public async Task PasswordUpdate_Method_Returns_BadRequest_On_IncorrectRequest()
        {
            var securityServiceMock = new Mock<ISecurityService>();

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.UpdatePassword(new PasswordUpdateRequest());

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestObjectResult.StatusCode);
            var passwordUpdateFailedResponse = Assert.IsAssignableFrom<PasswordUpdateResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Please make sure you send correct request, field can't be null", passwordUpdateFailedResponse.Errors);
        }

        [TestMethod]
        public async Task PasswordUpdate_Method_Returns_BadRequest_On_NotExistingUser()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.PasswordUpdateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new PasswordUpdateDto
                {
                    Errors = new[] { "This user does not exist anymore" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.UpdatePassword(new PasswordUpdateRequest
            {
                UserId = "520901e0-fe4e-4a06-9195-9f35ba05e094",
                Password = "TestPassword123!",
                Token = "CfDJ8JgR+bdQlp9AiexmNdwmv6Fo38Zi6LS1gfO/Ze2YXmV8QAWJos0oKWZ7FIrr+C2hKWVLgkMQ2Wr+zAiwk/z+Ow9f3rdThOSATiWIC8KVSraZvt7ZP2UR6dWPAZgZYZayPWGnGC6q2nc5NMPQQmIUFoiH+R9bMOnuM3bsML/sb7yAmXJILCLlcnH3qktSg9PauMExiY6eOnYzHIlm6een0aMHboXZ6lA1YtqtKQXd8RUB"
            });

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestObjectResult.StatusCode);
            var passwordUpdateFailedResponse = Assert.IsAssignableFrom<PasswordUpdateResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("This user does not exist anymore", passwordUpdateFailedResponse.Errors);
            Assert.True(passwordUpdateFailedResponse.CriticalError);
        }

        [TestMethod]
        public async Task PasswordUpdate_Method_Returns_BadRequest_On_ExpiredToken()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.PasswordUpdateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new PasswordUpdateDto
                {
                    Errors = new[] { "Expired link, please make another request" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.UpdatePassword(new PasswordUpdateRequest
            {
                UserId = "520901e0-fe4e-4a06-9195-9f35ba05e094",
                Password = "TestPassword123!",
                Token = "CfDJ8JgR+bdQlp9AiexmNdwmv6Fo38Zi6LS1gfO/Ze2YXmV8QAWJos0oKWZ7FIrr+C2hKWVLgkMQ2Wr+zAiwk/z+Ow9f3rdThOSATiWIC8KVSraZvt7ZP2UR6dWPAZgZYZayPWGnGC6q2nc5NMPQQmIUFoiH+R9bMOnuM3bsML/sb7yAmXJILCLlcnH3qktSg9PauMExiY6eOnYzHIlm6een0aMHboXZ6lA1YtqtKQXd8RUB"
            });

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestObjectResult.StatusCode);
            var passwordUpdateFailedResponse = Assert.IsAssignableFrom<PasswordUpdateResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Expired link, please make another request", passwordUpdateFailedResponse.Errors);
            Assert.False(passwordUpdateFailedResponse.CriticalError);
        }

        [TestMethod]
        public async Task PasswordUpdate_Method_Returns_OK_On_SuccessUpdate()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.PasswordUpdateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new PasswordUpdateDto
                {
                    Success = true
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.UpdatePassword(new PasswordUpdateRequest
            {
                UserId = "520901e0-fe4e-4a06-9195-9f35ba05e094",
                Password = "TestPassword123!",
                Token = "CfDJ8JgR+bdQlp9AiexmNdwmv6Fo38Zi6LS1gfO/Ze2YXmV8QAWJos0oKWZ7FIrr+C2hKWVLgkMQ2Wr+zAiwk/z+Ow9f3rdThOSATiWIC8KVSraZvt7ZP2UR6dWPAZgZYZayPWGnGC6q2nc5NMPQQmIUFoiH+R9bMOnuM3bsML/sb7yAmXJILCLlcnH3qktSg9PauMExiY6eOnYzHIlm6een0aMHboXZ6lA1YtqtKQXd8RUB"
            });

            var badRequestResult = Assert.IsType<NoContentResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);

            Assert.Equal(204, statusCode);
        }

        #endregion

        #region EmailConfirmationRequest

        [TestMethod]
        public async Task EmailConfirmationRequest_Method_Returns_BadRequest_On_IncorrectRequest()
        {
            var securityServiceMock = new Mock<ISecurityService>();

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.EmailConfirmationRequest(new EmailConfirmationRequest());

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestObjectResult.StatusCode);
            var emailConfirmationResponse = Assert.IsAssignableFrom<ConfirmationEmailResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Please make sure you send correct request, field can't be null", emailConfirmationResponse.Errors);
        }

        [TestMethod]
        public void EmailConfirmationRequest_Method_Returns_BadRequest_On_ModelStateFalse()
        {
            var securityServiceMock = new Mock<ISecurityService>();

            var controller = new SecurityController(securityServiceMock.Object);

            controller.ModelState.AddModelError("", "Model is incorrect");

            var result = controller.EmailConfirmationRequest(new EmailConfirmationRequest { Email = "WrongEmail" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<ConfirmationEmailResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.False(authFailedResponse.CriticalError);
            Assert.NotNull(authFailedResponse.Errors);
            Assert.NotEmpty(authFailedResponse.Errors);
        }

        [TestMethod]
        public async Task EmailConfirmationRequest_Method_Returns_BadRequest_On_ProblemWitSendingLinkToTheEmail()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.SendEmailConfirmationRequestAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new EmailConfirmationDto
                {
                    Errors = new[] { "Something went wrong, please try again or contact support" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.EmailConfirmationRequest(new EmailConfirmationRequest { Email = "sam.atkins@gmail.com" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var emailConfirmationResponse = Assert.IsAssignableFrom<ConfirmationEmailResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.False(emailConfirmationResponse.CriticalError);
            Assert.Contains("Something went wrong, please try again or contact support", emailConfirmationResponse.Errors);
        }

        [TestMethod]
        public async Task EmailConfirmationRequest_Method_Returns_OK_On_SuccessRequest()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.SendEmailConfirmationRequestAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new EmailConfirmationDto { Success = true }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.EmailConfirmationRequest(new EmailConfirmationRequest { Email = "sam.atkins@gmail.com" });

            var badRequestResult = Assert.IsType<NoContentResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);

            Assert.Equal(204, statusCode);
        }

        #endregion

        #region ConfirmEmail

        [TestMethod]
        public async Task ConfirmEmail_Method_Returns_BadRequest_On_IncorrectRequest()
        {
            var securityServiceMock = new Mock<ISecurityService>();

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.ConfirmEmail(new ConfirmEmailRequest());

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestObjectResult.StatusCode);
            var confirmEmailFailedRedResponse = Assert.IsAssignableFrom<ConfirmationEmailResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Please make sure you send correct request, field can't be null", confirmEmailFailedRedResponse.Errors);
        }

        [TestMethod]
        public async Task ConfirmEmail_Method_Returns_BadRequest_On_NotExistingUser()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new EmailConfirmationDto
                {
                    Errors = new[] { "This user does not exist anymore" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.ConfirmEmail(new ConfirmEmailRequest
            {
                UserId = "520901e0-fe4e-4a06-9195-9f35ba05e094",
                Token = "CfDJ8JgR+bdQlp9AiexmNdwmv6Fo38Zi6LS1gfO/Ze2YXmV8QAWJos0oKWZ7FIrr+C2hKWVLgkMQ2Wr+zAiwk/z+Ow9f3rdThOSATiWIC8KVSraZvt7ZP2UR6dWPAZgZYZayPWGnGC6q2nc5NMPQQmIUFoiH+R9bMOnuM3bsML/sb7yAmXJILCLlcnH3qktSg9PauMExiY6eOnYzHIlm6een0aMHboXZ6lA1YtqtKQXd8RUB"
            });

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestObjectResult.StatusCode);
            var confirmEmailFailedRedResponse = Assert.IsAssignableFrom<ConfirmationEmailResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("This user does not exist anymore", confirmEmailFailedRedResponse.Errors);
            Assert.True(confirmEmailFailedRedResponse.CriticalError);
        }

        [TestMethod]
        public async Task ConfirmEmail_Method_Returns_BadRequest_On_ExpiredToken()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new EmailConfirmationDto
                {
                    Errors = new[] { "Expired link, please make another request" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.ConfirmEmail(new ConfirmEmailRequest
            {
                UserId = "520901e0-fe4e-4a06-9195-9f35ba05e094",
                Token = "CfDJ8JgR+bdQlp9AiexmNdwmv6Fo38Zi6LS1gfO/Ze2YXmV8QAWJos0oKWZ7FIrr+C2hKWVLgkMQ2Wr+zAiwk/z+Ow9f3rdThOSATiWIC8KVSraZvt7ZP2UR6dWPAZgZYZayPWGnGC6q2nc5NMPQQmIUFoiH+R9bMOnuM3bsML/sb7yAmXJILCLlcnH3qktSg9PauMExiY6eOnYzHIlm6een0aMHboXZ6lA1YtqtKQXd8RUB"
            });

            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestObjectResult.StatusCode);
            var confirmEmailFailedRedResponse = Assert.IsAssignableFrom<ConfirmationEmailResponse>(badRequestObjectResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Expired link, please make another request", confirmEmailFailedRedResponse.Errors);
            Assert.False(confirmEmailFailedRedResponse.CriticalError);
        }

        [TestMethod]
        public async Task ConfirmEmail_Method_Returns_OK_On_SuccessUpdate()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new EmailConfirmationDto { Success = true }));

            var controller = new SecurityController(securityServiceMock.Object);

            var result = await controller.ConfirmEmail(new ConfirmEmailRequest
            {
                UserId = "520901e0-fe4e-4a06-9195-9f35ba05e094",
                Token = "CfDJ8JgR+bdQlp9AiexmNdwmv6Fo38Zi6LS1gfO/Ze2YXmV8QAWJos0oKWZ7FIrr+C2hKWVLgkMQ2Wr+zAiwk/z+Ow9f3rdThOSATiWIC8KVSraZvt7ZP2UR6dWPAZgZYZayPWGnGC6q2nc5NMPQQmIUFoiH+R9bMOnuM3bsML/sb7yAmXJILCLlcnH3qktSg9PauMExiY6eOnYzHIlm6een0aMHboXZ6lA1YtqtKQXd8RUB"
            });

            var badRequestResult = Assert.IsType<NoContentResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);

            Assert.Equal(204, statusCode);
        }

        #endregion

    }
}
