using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Common.Security
{
    /// <summary>
    /// Extens�es de autentica��o/JWT.
    /// Observa��o: Esta extens�o registra apenas o servi�o de gera��o de token.
    /// A configura��o do JwtBearer (AddAuthentication/AddJwtBearer) � feita no Program.cs.
    /// </summary>
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Registramos o servi�o respons�vel por gerar o token JWT.
            // O JwtTokenService depende de IConfiguration e ser� resolvido pelo DI.
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            return services;
        }
    }
}
