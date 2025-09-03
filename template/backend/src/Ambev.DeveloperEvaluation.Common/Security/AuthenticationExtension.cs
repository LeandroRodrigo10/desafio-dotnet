using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Common.Security
{
    /// <summary>
    /// Extensões de autenticação/JWT.
    /// Observação: Esta extensão registra apenas o serviço de geração de token.
    /// A configuração do JwtBearer (AddAuthentication/AddJwtBearer) é feita no Program.cs.
    /// </summary>
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Registramos o serviço responsável por gerar o token JWT.
            // O JwtTokenService depende de IConfiguration e será resolvido pelo DI.
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            return services;
        }
    }
}
