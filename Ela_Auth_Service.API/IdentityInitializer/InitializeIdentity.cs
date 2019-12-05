using System;
using System.Threading.Tasks;
using ELA_Auth_Service._Data;
using ELA_Auth_Service._Data._MySqlDataContext;
using ELA_Auth_Service._Data.ElaAuthDB;
using ELA_Auth_Service.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ELA_Auth_Service.IdentityInitializer
{
    public class InitializeIdentity
    {

        private readonly ElaAuthContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MySqlDataContext _mySqlDataContext;

        public InitializeIdentity(ElaAuthContext ctx, 
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager,
                MySqlDataContext mySqlDataContext)
        {
            _context = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _mySqlDataContext = mySqlDataContext;
        }

        public async Task Initialize()
        {
            if (!await _roleManager.RoleExistsAsync(DefaultIdentity.RoleAdmin))
                await _roleManager.CreateAsync(new IdentityRole(DefaultIdentity.RoleAdmin));

            if (!await _roleManager.RoleExistsAsync(DefaultIdentity.RoleUser))
                await _roleManager.CreateAsync(new IdentityRole(DefaultIdentity.RoleUser));

            if (await _userManager.FindByEmailAsync(DefaultIdentity.DefaultAdminUserName) is null)
            {
                var guid = Guid.NewGuid();
                var admin = new AppUser
                {
                    Id = guid.ToString(),
                    UserName = DefaultIdentity.DefaultAdminUserName,
                    Email = DefaultIdentity.DefaultAdminUserName,
                    EmailConfirmed = true,
                    Name = DefaultIdentity.DefaultAdminName
                };

                var creationResult = await _userManager.CreateAsync(admin, DefaultIdentity.DefaultAdminPassword);
                if (creationResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, DefaultIdentity.RoleAdmin);
                    await _userManager.AddToRoleAsync(admin, DefaultIdentity.RoleUser);
                }

                var addUserInMySqlDb = await _mySqlDataContext.CreateUser(guid, DefaultIdentity.DefaultAdminUserName, 10);

                if (!addUserInMySqlDb)
                    await _userManager.DeleteAsync(admin);
            }
        }
    }
}
