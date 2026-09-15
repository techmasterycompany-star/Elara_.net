using Elara.Application.Interfaces.Repository;
using Elara.Infrastructure.Data;
using Elara.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elara.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<DbSeeder>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            return services;
        }
    }
}
