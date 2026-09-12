using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elara.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
        this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(
                typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
