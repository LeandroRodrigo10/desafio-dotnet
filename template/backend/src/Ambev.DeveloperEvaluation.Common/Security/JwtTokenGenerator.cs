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
            _issuer = config["Jwt:Issuer"] ?? "app";
            _audience = config["Jwt:Audience"] ?? "app";
            _key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
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
