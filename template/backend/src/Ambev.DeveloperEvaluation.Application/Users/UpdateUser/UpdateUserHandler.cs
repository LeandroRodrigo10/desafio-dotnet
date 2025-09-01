#nullable enable

using AutoMapper;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Common.Security;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser
{
    /// <summary>
    /// Handles the UpdateUserCommand request
    /// </summary>
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateUserHandler(IUserRepository userRepository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateUserValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
            if (user is null)
                throw new KeyNotFoundException($"User with Id {command.Id} n�o encontrado");

            // Atualiza��o com preserva��o de valores existentes quando n�o enviados
            if (!string.IsNullOrWhiteSpace(command.Username))
                user.Username = command.Username.Trim();

            if (!string.IsNullOrWhiteSpace(command.Email))
                user.Email = command.Email.Trim();

            user.Phone = string.IsNullOrWhiteSpace(command.Phone)
                ? user.Phone
                : command.Phone.Trim();

            // Password: s� atualiza se informado
            if (!string.IsNullOrWhiteSpace(command.Password))
            {
                var hashed = _passwordHasher.HashPassword(command.Password!);
                user.Password = hashed;
            }

            // Role/Status: v�m obrigat�rios no comando
            user.Role = command.Role;
            user.Status = command.Status;

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            // Mapeia para resultado (consumido pela WebApi, que converte Username -> Name)
            return _mapper.Map<UpdateUserResult>(user);
        }
    }
}
