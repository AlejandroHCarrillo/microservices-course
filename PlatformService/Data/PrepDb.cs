using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"!!! Could not run migrations: { ex.Message}" );
                }
                finally
                {
                    Console.ResetColor();
                }
            }

            if (!context.Platforms.Any())
            {
                Console.ForegroundColor= ConsoleColor.Green;
                Console.WriteLine("--> Seeding data...");
                context.Platforms.AddRange(
                    new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name="SQL Server Express", Publisher="Microsoft", Cost="Free" }, 
                    new Platform() { Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free" } 
                    );
                context.SaveChanges();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("--> We already have data");
            }
            Console.ResetColor();
        }
    }
}
