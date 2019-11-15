using System.Threading.Tasks;
using ELA_Auth_Service.Domain.DTO;

namespace ELA_Auth_Service.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationDto> RegisterAsync(string email, string password, string name);

        Task<AuthenticationDto> LoginAsync(string email, string password); 

        Task<AuthenticationDto> RefreshTokenAsync(string token, string refreshToken);
    }
}
