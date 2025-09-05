using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ambev.DeveloperEvaluation.Common.Security
{
    public interface IJwtTokenService
    {
        string Generate(Guid userId, string? email, string? username, string roleName);
    }

    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _expiresMinutes;

        public JwtTokenService(IConfiguration config)
        {
            // Garantir não-nulidade com fallback seguro (igual ao Program.cs)
            var issuerCfg = config["Jwt:Issuer"];
            _issuer = string.IsNullOrWhiteSpace(issuerCfg) ? "Ambev.DeveloperEvaluation" : issuerCfg!;

            var audienceCfg = config["Jwt:Audience"];
            _audience = string.IsNullOrWhiteSpace(audienceCfg) ? "Ambev.DeveloperEvaluation.Clients" : audienceCfg!;

            var keyCfg = config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyCfg))
            {
                var env = config["ASPNETCORE_ENVIRONMENT"] ?? "Production";
                if (env.Equals("Development", StringComparison.OrdinalIgnoreCase))
                {
                    // Somente DEV — mantenha sincronizado com Program.cs
                    keyCfg = "dev-super-secret-key-change-me-0123456789-ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                }
                else
                {
                    throw new InvalidOperationException("Jwt:Key not configured");
                }
            }
            _key = keyCfg!;

            _expiresMinutes = int.TryParse(config["Jwt:ExpiresMinutes"], out var m) ? m : 60;
        }

        public string Generate(Guid userId, string? email, string? username, string roleName)
        {
            if (userId == Guid.Empty) throw new ArgumentException("userId is required", nameof(userId));
            roleName ??= "User";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email ?? string.Empty),
                new Claim(ClaimTypes.Name, username ?? string.Empty),
                new Claim(ClaimTypes.Role, roleName)
            };

            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
