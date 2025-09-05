using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Ambev.DeveloperEvaluation.Common.Security; // usar serviço central de JWT

namespace Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser
{
    public sealed class AuthenticateUserHandler : IRequestHandler<AuthenticateUserRequest, AuthenticateUserResponse>
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenService _tokens;

        public AuthenticateUserHandler(IUserRepository users, IJwtTokenService tokens)
        {
            _users = users;
            _tokens = tokens;
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

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

            var token = _tokens.Generate(user.Id, user.Email, null, user.Role.ToString());

            return new AuthenticateUserResponse
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
