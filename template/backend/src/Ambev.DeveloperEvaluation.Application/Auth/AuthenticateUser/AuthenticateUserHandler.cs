using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Enums;        // UserStatus
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser
{
    public sealed class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPasswordHasher _passwordHasher;

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
            var emailInput = request.Email?.Trim();
            if (string.IsNullOrWhiteSpace(emailInput) || string.IsNullOrWhiteSpace(request.Password))
                throw new UnauthorizedAccessException("Invalid email or password");

            // tenta com o email como veio e também normalizado em lower-case
            var normalizedEmail = emailInput.ToLowerInvariant();
            var user =
                await _userRepository.GetByEmailAsync(emailInput, cancellationToken)
                ?? await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // bloqueia quem não estiver ativo
            if (user.Status != UserStatus.Active)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Ajuste crítico: muitas libs esperam Verify(hash, plain)
            var passwordOk = _passwordHasher.Verify(user.Password, request.Password);
            if (!passwordOk)
                throw new UnauthorizedAccessException("Invalid email or password");

            var token = _jwtTokenService.Generate(
                user.Id,
                user.Email,
                user.Username,
                user.Role.ToString()
            );

            return new AuthenticateUserResult
            {
                Token = token,
                Name = user.Username ?? string.Empty,
                Role = user.Role.ToString()
            };
        }
    }
}
