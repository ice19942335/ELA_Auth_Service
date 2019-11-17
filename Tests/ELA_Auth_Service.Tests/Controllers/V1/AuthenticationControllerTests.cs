using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1.Requests.Identity.Auth;
using ELA_Auth_Service.Contracts.V1.Responses.Identity.Auth;
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
    public class AuthenticationControllerTests
    {

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

            Assert.Equal(400, statusCode);
            Assert.False(authFailedResponse.CriticalError);
            Assert.NotNull(authFailedResponse.Errors);
            Assert.NotEmpty(authFailedResponse.Errors);
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

            var successRequestResult = Assert.IsType<OkObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(successRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthSuccessResponse>(successRequestResult.Value);

            Assert.Equal(200, statusCode);
            Assert.NotNull(authResponse.Token);
            Assert.NotNull(authResponse.RefreshToken);
        }

        #endregion

        #region Login

        [TestMethod]
        public void Login_Method_Returns_BadRequest_On_IncorrectRequest()
        {
            var authServiceMock = new Mock<IAuthenticationService>();

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Login(new UserLoginRequest());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Please make sure you send correct request, field can't be null", authResponse.Errors);
        }

        [TestMethod]
        public async Task Login_Method_Returns_BadRequest_On_ModelStateFalse()
        {
            var authServiceMock = new Mock<IAuthenticationService>();

            var controller = new AuthenticationController(authServiceMock.Object);

            controller.ModelState.AddModelError("", "Model is incorrect");

            var result = await controller.Login(new UserLoginRequest
            {
                Email = "WrongEmail",
                Password = "WrongPassword"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authFailedResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.False(authFailedResponse.CriticalError);
            Assert.NotEmpty(authFailedResponse.Errors);
        }

        [TestMethod]
        public void Login_Method_Returns_BadRequest_On_UserNotExist()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "User does not exist" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Login(new UserLoginRequest
            {
                Email = "testEmail@Gmail.com",
                Password = "TestPassword123!"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("User does not exist", authResponse.Errors);
            Assert.False(authResponse.CriticalError);
        }

        [TestMethod]
        public void Login_Method_Returns_BadRequest_On_PasswordIsIncorrect()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "Password is incorrect" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Login(new UserLoginRequest
            {
                Email = "testEmail@Gmail.com",
                Password = "TestPassword123"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Password is incorrect", authResponse.Errors);
            Assert.False(authResponse.CriticalError);
        }

        [TestMethod]
        public void Login_Method_Returns_OK_On_Success_Login()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                    RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2",
                    Success = true
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.Login(new UserLoginRequest
            {
                Email = "testEmail@gmail.com",
                Password = "TestPassword123!"
            });

            var successRequestResult = Assert.IsType<OkObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(successRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthSuccessResponse>(successRequestResult.Value);

            Assert.Equal(200, statusCode);
            Assert.NotNull(authResponse.Token);
            Assert.NotNull(authResponse.RefreshToken);
        }

        #endregion

        #region Refresh

        [TestMethod]
        public void Refresh_Method_Returns_BadRequest_On_IncorrectRequest()
        {
            var authServiceMock = new Mock<IAuthenticationService>();

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest());

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Please make sure you send correct request, field can't be null", authResponse.Errors);
        }

        [TestMethod]
        public void Refresh_Method_Returns_BadRequest_On_InvalidToken()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "Invalid Token" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("Invalid Token", authResponse.Errors);
            Assert.True(authResponse.CriticalError);
        }

        [TestMethod]
        public void Refresh_Method_Returns_BadRequest_On_RefreshTokenNotExist()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "This refresh token does not exist" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("This refresh token does not exist", authResponse.Errors);
            Assert.True(authResponse.CriticalError);
        }

        [TestMethod]
        public void Refresh_Method_Returns_BadRequest_On_RefreshTokenHasBeenInvalidated()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "This refresh token has been invalidated" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("This refresh token has been invalidated", authResponse.Errors);
            Assert.True(authResponse.CriticalError);
        }

        [TestMethod]
        public void Refresh_Method_Returns_BadRequest_On_RefreshTokenHasNotExpiredJet()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "This token hasn't expired jet" },
                    CriticalError = false,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("This token hasn't expired jet", authResponse.Errors);
            Assert.False(authResponse.CriticalError);
        }

        [TestMethod]
        public void Refresh_Method_Returns_BadRequest_On_RefreshTokenHasBeenUsed()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "This refresh token has been used" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("This refresh token has been used", authResponse.Errors);
            Assert.True(authResponse.CriticalError);
        }

        [TestMethod]
        public void Refresh_Method_Returns_BadRequest_On_RefreshTokenHasExpired()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "This refresh token has expired" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("This refresh token has expired", authResponse.Errors);
            Assert.True(authResponse.CriticalError);
        }

        [TestMethod]
        public void Refresh_Method_Returns_BadRequest_On_RefreshTokenDoesNotMuchProvidedJWT()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Errors = new[] { "This refresh token does not match this JWT" },
                    CriticalError = true,
                    Success = false
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2"
            });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(badRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthFailedResponse>(badRequestResult.Value);

            Assert.Equal(400, statusCode);
            Assert.Contains("This refresh token does not match this JWT", authResponse.Errors);
            Assert.True(authResponse.CriticalError);
        }

        [TestMethod]
        public void Refresh_Method_Returns_OK_On_Success_Refresh()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new AuthenticationDto
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                    RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2",
                    Success = true
                }));

            var controller = new AuthenticationController(authServiceMock.Object);

            var result = controller.RefreshToken(new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJpY2UxOTk0MjMzNUBnbWFpbC5jb20iLCJqdGkiOiI4YmFlZjMxNS02MjMxLTRmMWItOWQ4YS1hMWQ4MmI2NWQwMzMiLCJlbWFpbCI6ImljZTE5OTQyMzM1QGdtYWlsLmNvbSIsImlkIjoiZTdlOTRjMTEtMjBkNy00NjAyLWJjNzAtMGYwMTVjZDA0MWJkIiwibmJmIjoxNTcyOTUwMDc5LCJleHAiOjE1NzI5NTA5NzksImlhdCI6MTU3Mjk1MDA3OX0.9mR8PcLySEvLqZOJZQedbjepb2vs4s4CZCBBdDYlRpM",
                RefreshToken = "e3107d7b-f27d-4025-8c1d-81d1471b04f2"
            });

            var successRequestResult = Assert.IsType<OkObjectResult>(result.Result);
            var statusCode = Assert.IsAssignableFrom<int>(successRequestResult.StatusCode);
            var authResponse = Assert.IsAssignableFrom<AuthSuccessResponse>(successRequestResult.Value);

            Assert.Equal(200, statusCode);
            Assert.NotNull(authResponse.Token);
            Assert.NotNull(authResponse.RefreshToken);
        }

        #endregion
    }
}
