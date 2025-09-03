using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser
{
    public sealed class AuthenticateUserHandler : IRequestHandler<AuthenticateUserRequest, AuthenticateUserResponse>
    {
        private readonly IUserRepository _users;
        private readonly IConfiguration _config;

        public AuthenticateUserHandler(IUserRepository users, IConfiguration config)
        {
            _users = users;
            _config = config;
        }

        public async Task<AuthenticateUserResponse> Handle(AuthenticateUserRequest request, CancellationToken ct)
        {
            var email = request.Email?.Trim().ToLowerInvariant();
            var password = request.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            var user = await _users.GetByEmailAsync(email, ct);
            if (user is null)
                throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

            // Verificação de senha (ajuste se sua entidade usar PasswordHash)
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

            var token = BuildJwtFromConfig(_config, user);

            return new AuthenticateUserResponse
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        private static string BuildJwtFromConfig(IConfiguration config, User user)
        {
            string? issuer = config["Jwt:Issuer"];
            string? audience = config["Jwt:Audience"];
            string? key = config["Jwt:Key"];

            issuer ??= "TestIssuer";
            audience ??= "TestAudience";
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Jwt:Key não configurado.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow.AddMinutes(-1),
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
