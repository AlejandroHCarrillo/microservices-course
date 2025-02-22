using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }

        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Este código establece una relación entre Platform y Command
            // donde una Platform puede tener muchos Commands,
            // y cada Command tiene una clave foránea PlatformId
            // que hace referencia a su Platform correspondiente.
            modelBuilder.Entity<Platform>()
                .HasMany(p => p.Commands)
                .WithOne(p => p.Platform!)
                .HasForeignKey(p => p.PlatformId);

            // Este código establece una relación entre Command y Platform
            // donde cada Command está asociado con una Platform,
            // y una Platform puede tener muchos Commands.
            // La clave foránea PlatformId en Command se utiliza para mantener esta relación.
            modelBuilder.Entity<Command>()
                .HasOne(p => p.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(p => p.PlatformId);

        }
    }
}
