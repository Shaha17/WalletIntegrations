using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ewallet_integration.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ewallet_integration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                var context = services.GetRequiredService<WalletDbContext>();
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrate successful");

                await ContextHelper.Seed(context);
                logger.LogInformation("Seeding successful");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}