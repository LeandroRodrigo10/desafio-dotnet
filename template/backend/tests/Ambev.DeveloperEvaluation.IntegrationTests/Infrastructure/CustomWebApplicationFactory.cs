using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ambev.DeveloperEvaluation.ORM;

namespace Ambev.DeveloperEvaluation.IntegrationTests.Infrastructure
{
    /// <summary>
    /// Sobe a WebApi em memória para testes de integração,
    /// substituindo o PostgreSQL por EF Core InMemory e injetando config JWT.
    /// </summary>
    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Força ambiente "Development"
            builder.UseEnvironment("Development");

            // 🔑 Injeta configuração em memória para JWT, evitando 500 no /api/Auth
            builder.ConfigureAppConfiguration(config =>
            {
                var mem = new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = "test-issuer",
                    ["Jwt:Audience"] = "test-audience",
                    // chave de 32+ chars para HMAC-SHA256
                    ["Jwt:Key"] = "super-secret-test-key-1234567890-abcdef",
                    ["Jwt:ExpiresMinutes"] = "60",
                    // Opcional: ConnectionString dummy (não será usada com InMemory)
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test"
                };
                config.AddInMemoryCollection(mem!);
            });

            builder.ConfigureServices(services =>
            {
                // Remove a configuração original do DbContext (PostgreSQL)
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DefaultContext>)
                );
                if (dbContextDescriptor is not null)
                    services.Remove(dbContextDescriptor);

                // Adiciona o DbContext InMemory para os testes
                services.AddDbContext<DefaultContext>(options =>
                {
                    options.UseInMemoryDatabase($"IntegrationTestsDb_{Guid.NewGuid()}");
                });

                // Garante que o banco in-memory está criado
                using var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                ctx.Database.EnsureCreated();
            });

            return base.CreateHost(builder);
        }
    }
}
