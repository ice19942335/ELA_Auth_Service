using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ELA_Auth_Service.Data;
using ELA_Auth_Service.Data._MySqlDataContext;
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
    //This class using real DI pipeline with real Database
    [TestClass]
    public class AuthenticationServiceTests
    {
        private AuthenticationService _authenticationService;
        private Mock<UserManager<AppUser>> _userManagerMock;

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
            var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var mySqlDataContext = serviceScope.ServiceProvider.GetRequiredService<MySqlDataContext>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<AuthenticationService>>();


            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Object.UserValidators.Add(new UserValidator<AppUser>());
            _userManagerMock.Object.PasswordValidators.Add(new PasswordValidator<AppUser>());
  

            _authenticationService = new AuthenticationService(
                securityService,
                emailService,
                configuration,
                _userManagerMock.Object,
                jwtSettings,
                tokenValidationParameters,
                context,
                roleManager,
                mySqlDataContext,
                logger);
        }

        [TestMethod]
        public async Task RegisterAsync_Method_Returns_AuthenticationDto_SuccessEqualFalse_And_ErrorList_On_ExistingUser()
        {
            //Arrange
            var email = "sam.atkins@gmail.com";
            var password = "Password123!";
            var name = "Sam";

            var expectedErrorList = new[] { "User with this email address already exists" };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new AppUser()));

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
    }
}
