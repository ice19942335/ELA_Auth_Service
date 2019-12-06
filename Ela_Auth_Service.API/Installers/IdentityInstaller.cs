using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELA_Auth_Service._Data.ElaAuthDB;
using ELA_Auth_Service.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELA_Auth_Service.Installers
{
    public class IdentityInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultIdentity<AppUser>(options => { options.Password.RequireNonAlphanumeric = false; })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ElaAuthContext>();
        }
    }
}
