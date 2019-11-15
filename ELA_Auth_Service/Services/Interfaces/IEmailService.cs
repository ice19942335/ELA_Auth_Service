using System.Threading.Tasks;
using ELA_Auth_Service.Domain.DTO;

namespace ELA_Auth_Service.Services.Interfaces
{
    public interface IEmailService
    {
        Task<PasswordUpdateDto> SendEmailAsync(string email, string subject, string message);
    }
}
