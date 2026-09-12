using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace Elara.Infrastructure.Data
{
    public class AppDbContextFactory
        : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var envPath = Path.Combine(basePath, ".env");
            if (!File.Exists(envPath))
                envPath = Path.Combine(basePath, "src", "Elara.API", ".env");
            if (!File.Exists(envPath))
                envPath = Path.Combine(basePath, "Elara.API", ".env");

            if (File.Exists(envPath))
                Env.Load(envPath);

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? throw new InvalidOperationException(
                    $"Connection string 'DefaultConnection' not found. Tried paths: .env, src/Elara.API/.env, Elara.API/.env");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
