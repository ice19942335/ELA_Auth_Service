using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELA_Auth_Service.Installers
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}
