using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser
{
    public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            // Normalização (locals para agradar o analisador de nullability)
            var newEmail = command.Email?.Trim().ToLower();
            var newUsername = command.Username?.Trim();
            var newPhone = command.Phone?.Trim();
            var newPassword = command.Password?.Trim();

            // Checar se o usuário existe
            var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
            if (user is null)
                throw new ValidationException($"User with Id '{command.Id}' not found");

            // Checar unicidade do e-mail (se informado)
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                var existingUser = await _userRepository.GetByEmailAsync(newEmail, cancellationToken);
                if (existingUser != null && existingUser.Id != command.Id)
                    throw new ValidationException("Email already in use by another user");
            }

            // Atualizar os campos somente se informados
            if (!string.IsNullOrWhiteSpace(newUsername))
                user.Username = newUsername; 

            if (!string.IsNullOrWhiteSpace(newEmail))
                user.Email = newEmail; 

            if (!string.IsNullOrWhiteSpace(newPhone))
                user.Phone = newPhone;

            if (!string.IsNullOrWhiteSpace(newPassword))
                user.Password = newPassword;

            user.Role = command.Role;
            user.Status = command.Status;

            // Salvar
            await _userRepository.UpdateAsync(user, cancellationToken);

            return new UpdateUserResult
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                Status = user.Status.ToString()
            };
        }
    }
}
