using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELA_Auth_Service.Data;
using ELA_Auth_Service.Domain.DTO;
using ELA_Auth_Service.Domain.Entities;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ELA_Auth_Service.Services.Implementation
{
    public class SecurityService : ISecurityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly string _clientUrl;
        private readonly DataContext _dataContext;
        private readonly IEmailService _emailService;

        public SecurityService(IConfiguration configuration, UserManager<AppUser> userManager, DataContext dataContext, IEmailService emailService)
        {
            _userManager = userManager;
            _dataContext = dataContext;
            _emailService = emailService;
            _clientUrl = configuration["ClientSidePasswordResetPath"];
        }

        public async Task<PasswordUpdateDto> PasswordResetRequestAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return new PasswordUpdateDto { Errors = new[] { "User with this email does not exist" }, CriticalError = true };

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return new PasswordUpdateDto { Errors = new[] { "User email is not confirmed, sorry, we can't help you with this" } };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callBackUrl = $"<a href='{_clientUrl}?userId={user.Id}&token={token}'>Click this link to reset password</a>";

            var sendEmailResult = await _emailService.SendEmailAsync(user.Email, "ELA Password reset", callBackUrl);

            if (!sendEmailResult.Success)
                return new PasswordUpdateDto { Errors = new[] { "Something went wrong, please try again or contact support" } };

            return new PasswordUpdateDto { Success = true };
        }

        public async Task<PasswordUpdateDto> PasswordUpdateAsync(string userId, string password, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return new PasswordUpdateDto { Errors = new[] { "This user does not exist anymore" }, CriticalError = true };

            var passwordUpdateResult = await _userManager.ResetPasswordAsync(user, token, password);

            if (!passwordUpdateResult.Succeeded)
            {
                //Valid token
                if (passwordUpdateResult.Errors.FirstOrDefault(x => x.Description == "Invalid token.") is null)
                    return new PasswordUpdateDto { Errors = passwordUpdateResult.Errors.Select(x => x.Description) };
                //-----------

                //Invalid token
                return new PasswordUpdateDto { Errors = new[] { "Expired link, please make another request" } };
                //-------------
            }

            var refreshToken = _dataContext.RefreshTokens.FirstOrDefault(x => x.UserId == user.Id);
            if (refreshToken != null) refreshToken.Invalidated = true;

            await _dataContext.SaveChangesAsync();

            return new PasswordUpdateDto { Success = true };
        }

        public async Task<EmailConfirmationDto> SendEmailConfirmationRequestAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return new EmailConfirmationDto { Errors = new[] { "User does not exist anymore" } };

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callBackUrl = $"<a href='{_clientUrl}?userId={user.Id}&token={token}'>Click this link to confirm e-mail</a>";

            var sendEmailConfirmationLinkResult = await _emailService.SendEmailAsync(user.Email, "ELA E-mail confirmation", callBackUrl);

            if (!sendEmailConfirmationLinkResult.Success)
                return new EmailConfirmationDto { Errors = new[] { "Something went wrong, please try again or contact support" } };

            return new EmailConfirmationDto { Success = true };
        }

        public async Task<EmailConfirmationDto> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return new EmailConfirmationDto
                {
                    Errors = new[] { "This user does not exist anymore" },
                    CriticalError = true
                };

            var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, token);

            if (!emailConfirmationResult.Succeeded)
            {
                //Valid token
                if (emailConfirmationResult.Errors.FirstOrDefault(x => x.Description == "Invalid token.") is null)
                    return new EmailConfirmationDto { Errors = emailConfirmationResult.Errors.Select(x => x.Description) };
                //-----------

                //Invalid token
                return new EmailConfirmationDto { Errors = new[] { "Expired link, please make another request" } };
                //-------------
            }

            return new EmailConfirmationDto { Success = true };
        }
    }
}
