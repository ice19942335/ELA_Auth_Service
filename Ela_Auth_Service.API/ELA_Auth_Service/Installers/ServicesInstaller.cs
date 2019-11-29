using ELA_Auth_Service.Options;
using ELA_Auth_Service.Services.Implementation;
using ELA_Auth_Service.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELA_Auth_Service.Installers
{
    public class ServicesInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //Scoped
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ISecurityService, SecurityService>();

            //Transient
            services.AddTransient<IEmailService, EmailService>();

            //SingleTone
            var sendGridSettings = new SendGridSettings();
            configuration.Bind(nameof(SendGridSettings), sendGridSettings);
            services.AddSingleton(sendGridSettings);
        }
    }
}
