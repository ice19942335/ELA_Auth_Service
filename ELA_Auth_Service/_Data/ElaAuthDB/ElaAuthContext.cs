using ELA_Auth_Service.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ELA_Auth_Service._Data.ElaAuthDB
{
    public class ElaAuthContext : IdentityDbContext<AppUser>
    {
        public ElaAuthContext(DbContextOptions<ElaAuthContext> options)
            : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }  

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
