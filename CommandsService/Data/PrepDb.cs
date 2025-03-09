using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder) {
            using (var servicescope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = servicescope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient.ReturnAllPlatforms();
                SeedData(servicescope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("--> Seeding new Platforms...");
            Console.ResetColor();
            foreach (var plat in platforms) {
                if (!repo.ExternalPlatExists(plat.ExternalID)) {
                    repo.CreatePlatform(plat);
                }
                repo.SaveChanges();
            }

        }
    }
}
