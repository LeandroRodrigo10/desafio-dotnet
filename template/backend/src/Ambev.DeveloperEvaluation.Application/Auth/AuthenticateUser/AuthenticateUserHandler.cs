using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser
{
    /// <summary>
    /// Handler responsável por autenticar o usuário.
    /// </summary>
    public sealed class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPasswordHasher _passwordHasher;

        private const string InvalidCredentialsMessage = "Invalid email or password";

        public AuthenticateUserHandler(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<AuthenticateUserResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            // normaliza e-mail e valida entrada
            var email = request.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
                throw new UnauthorizedAccessException(InvalidCredentialsMessage);

            // busca usuário por e-mail normalizado
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
                throw new UnauthorizedAccessException(InvalidCredentialsMessage);

            // exige usuário ativo
            if (user.Status != UserStatus.Active)
                throw new UnauthorizedAccessException(InvalidCredentialsMessage);

            // confere senha (ordem: plaintext, hash)
            var passwordOk = _passwordHasher.Verify(request.Password, user.Password);
            if (!passwordOk)
                throw new UnauthorizedAccessException(InvalidCredentialsMessage);

            // gera token JWT
            var token = _jwtTokenService.Generate(user.Id, user.Email, user.Username, user.Role.ToString());

            return new AuthenticateUserResult
            {
                Token = token,
                Name = user.Username ?? string.Empty,
                Role = user.Role.ToString()
            };
        }
    }
}
