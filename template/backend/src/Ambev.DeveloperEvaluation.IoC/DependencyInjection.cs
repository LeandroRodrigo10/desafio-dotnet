using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ambev.DeveloperEvaluation.IoC
{
    public static class DependencyInjection
    {
        // antes (provável):
        // public static void AddDependencies(this WebApplicationBuilder builder)

        // depois:
        public static IServiceCollection AddDependencies(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment env)
        {
            // Onde tinha builder.Services => usar "services"
            // Onde tinha builder.Configuration => usar "configuration"
            // Onde tinha builder.Environment => usar "env"

            // EXEMPLOS (ajuste para o que você já registra hoje):
            // services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            // services.AddMediatR(typeof(SomeHandler).Assembly);
            // services.AddDbContext<AppDbContext>(o => o.UseNpgsql(configuration.GetConnectionString("Default")));

            return services;
        }
    }
}
