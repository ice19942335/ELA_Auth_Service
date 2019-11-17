using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELA_Auth_Service.Domain.DTO;

namespace ELA_Auth_Service.Services.Interfaces
{
    public interface ISecurityService
    {
        Task<PasswordUpdateDto> PasswordResetRequestAsync(string email);

        Task<PasswordUpdateDto> PasswordUpdateAsync(string userId, string password, string token);

        Task<EmailConfirmationDto> SendEmailConfirmationRequestAsync(string email);

        Task<EmailConfirmationDto> ConfirmEmailAsync(string userId, string token);
    }
}
