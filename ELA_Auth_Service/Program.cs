using System;
using System.IO;
using System.Threading.Tasks;
using ELA_Auth_Service._Data._MySqlDataContext;
using ELA_Auth_Service._Data.ElaAuthDB;
using ELA_Auth_Service.Domain.Entities;
using ELA_Auth_Service.IdentityInitializer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ELA_Auth_Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var path = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOG"), "ELA_AUTH_LOG.txt"); //ELA_AUTH_LOG.txt
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path,
                    fileSizeLimitBytes: 1_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                using (var serviceScope = host.Services.CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ElaAuthContext>();

                    await dbContext.Database.MigrateAsync();

                    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                    var mySql = serviceScope.ServiceProvider.GetRequiredService<MySqlDataContext>();

                    var baseIdentityInitializer = new InitializeIdentity(dbContext, userManager, roleManager, mySql);
                    await baseIdentityInitializer.Initialize();
                }

                await host.RunAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
