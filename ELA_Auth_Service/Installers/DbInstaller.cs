using ELA_Auth_Service.Data;
using ELA_Auth_Service.Data._MySqlDataContext;
using ELA_Auth_Service.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELA_Auth_Service.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                //options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
                options.UseSqlServer(configuration.GetConnectionString("AuthProductionConnection")));
            services.AddDefaultIdentity<AppUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<DataContext>();

            services.AddTransient<MySqlDataContext>(_ => new MySqlDataContext(configuration.GetConnectionString("DataServiceConnection")));
        }
    }
}
