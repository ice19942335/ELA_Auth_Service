using System.Threading.Tasks;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Auth;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Email;
using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Password;
using ELA_Auth_Service.Domain.DTO;

namespace ELA_Auth_Service.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthenticationDto> RegisterAsync(UserRegistrationRequest request);

        Task<AuthenticationDto> LoginAsync(string email, string password);

        Task<AuthenticationDto> RefreshTokenAsync(string token, string refreshToken);

        Task<PasswordUpdateDto> PasswordResetRequestAsync(string email);

        Task<PasswordUpdateDto> PasswordUpdateAsync(PasswordUpdateRequest request);

        Task<EmailConfirmationDto> SendEmailConfirmationRequestAsync(EmailConfirmationRequest request);

        Task<EmailConfirmationDto> ConfirmEmailAsync(ConfirmEmailRequest request);
    }
}
