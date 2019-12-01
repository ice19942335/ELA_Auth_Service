using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELA_Auth_Service._Data._MySqlDataContext;
using ELA_Auth_Service._Data.ElaAuthDB;
using ELA_Auth_Service.Domain.DTO;
using ELA_Auth_Service.Domain.Entities;
using ELA_Auth_Service.Options;
using ELA_Auth_Service.Services.Implementation;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit.Sdk;

using Assert = Xunit.Assert;

namespace ELA_Auth_Service.UnitTests.Services.Implementation
{
    /// <summary>
    /// This class using real DI pipeline with real Database
    /// </summary>
    [TestClass]
    public class AuthenticationServiceTests
    {
        private AuthenticationService _authenticationService;

        [TestInitialize]
        public void Initialize()
        {
            string[] args = new string[] { };

            var host = Program.CreateHostBuilder(args).Build();
            var serviceScope = host.Services.CreateScope();

            var securityService = serviceScope.ServiceProvider.GetRequiredService<ISecurityService>();
            var emailService = serviceScope.ServiceProvider.GetRequiredService<IEmailService>();
            var configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var jwtSettings = serviceScope.ServiceProvider.GetRequiredService<JwtSettings>();
            var tokenValidationParameters = serviceScope.ServiceProvider.GetRequiredService<TokenValidationParameters>();
            var context = serviceScope.ServiceProvider.GetRequiredService<ElaAuthContext>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var mySqlDataContext = serviceScope.ServiceProvider.GetRequiredService<MySqlDataContext>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<AuthenticationService>>();

            _authenticationService = new AuthenticationService(
                securityService,
                emailService,
                configuration,
                userManager,
                jwtSettings,
                tokenValidationParameters,
                context,
                roleManager,
                mySqlDataContext,
                logger);
        }

        [TestMethod]
        public async Task RegisterAsync_Method_Returns_Error_On_ExistingUser()
        {
            //Arrange
            var email = "sam.atkins@gmail.com";
            var password = "Password123!";
            var name = "Sam";

            var expectedErrorList = new[] { "User with this email address already exists" };

            //Act
            var result = await _authenticationService.RegisterAsync(email, password, name);

            //Assert

            var authenticationDtoResult = Assert.IsAssignableFrom<AuthenticationDto>(result);
            Assert.Equal(expectedErrorList, authenticationDtoResult.Errors);
            Assert.False(authenticationDtoResult.CriticalError);
            Assert.False(authenticationDtoResult.Success);
            Assert.Null(authenticationDtoResult.Token);
            Assert.Null(authenticationDtoResult.RefreshToken);
        }

        [TestMethod]
        public async Task RegisterAsync_Method_Returns_Error_On_PasswordDoesNotMeetTheRequirements()
        {
            //Arrange
            var email = "sam.atkins.unique.email.never.being.registred@gmail.com";
            var password = "Password123";
            var name = "Sam";

            var expectedErrorList = new[]
            {
                "Passwords must be at least 6 characters.",
                "Passwords must have at least one non alphanumeric character.",
                "Passwords must have at least one digit ('0'-'9').",
                "Passwords must have at least one lowercase ('a'-'z').",
                "Passwords must have at least one uppercase ('A'-'Z').",
                "Passwords must use at least 1 different characters."
            };

            //Act
            var result = await _authenticationService.RegisterAsync(email, password, name);

            //Assert
            var authenticationDtoResult = Assert.IsAssignableFrom<AuthenticationDto>(result);

            foreach (var error in authenticationDtoResult.Errors.ToArray())
                Assert.Contains(error, expectedErrorList);
            
            Assert.False(authenticationDtoResult.CriticalError);
            Assert.False(authenticationDtoResult.Success);
            Assert.Null(authenticationDtoResult.Token);
            Assert.Null(authenticationDtoResult.RefreshToken);
        }

        [TestMethod]
        public async Task RegisterAsync_Method_Returns_Error_On_ProblemOnWritingInToDataServiceDB()
        {
            //Arrange
            var email = "sam.atkins.unique.email.never.being.registred@gmail.com";
            var password = "Password123!";
            var name = "SamaaaaaaaSamaaaaaaaSamaaaaaaaSamaaaaaaaSamaaaaaaa";

            var expectedErrorList = new[] 
            {
                "Problem on writing entry in DATA SERVICE DB"
            };

            //Act
            var result = await _authenticationService.RegisterAsync(email, password, name);

            //Assert
            var authenticationDtoResult = Assert.IsAssignableFrom<AuthenticationDto>(result);

            foreach (var error in authenticationDtoResult.Errors.ToArray())
                Assert.Contains(error, expectedErrorList);

            Assert.True(authenticationDtoResult.CriticalError);
            Assert.False(authenticationDtoResult.Success);
            Assert.Null(authenticationDtoResult.Token);
            Assert.Null(authenticationDtoResult.RefreshToken);
        }

        [TestMethod]
        public async Task RegisterAsync_Method_Returns_Success_And_Tokens()
        {
            //Arrange
            var email = $"AutoTestUser_{Guid.NewGuid().ToString().Substring(0, 8)}@mail.com";
            var password = "Password123!";
            var name = "AutoTestUser";

            //Act
            var result = await _authenticationService.RegisterAsync(email, password, name);

            //Assert
            var authenticationDtoResult = Assert.IsAssignableFrom<AuthenticationDto>(result);

            Assert.False(authenticationDtoResult.CriticalError);
            Assert.True(authenticationDtoResult.Success);
            Assert.NotNull(authenticationDtoResult.Token);
            Assert.NotNull(authenticationDtoResult.RefreshToken);
        }
    }
}
