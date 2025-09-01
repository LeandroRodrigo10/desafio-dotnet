using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Common.Security;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser
{
    /// <summary>
    /// Handler for processing CreateUserCommand requests
    /// </summary>
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        /// <summary>
        /// Initializes a new instance of CreateUserHandler
        /// </summary>
        public CreateUserHandler(IUserRepository userRepository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Handles the CreateUserCommand request
        /// </summary>
        public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateUserCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException($"User with email {command.Email} already exists");

            var user = _mapper.Map<User>(command);
            user.Password = _passwordHasher.HashPassword(command.Password);

            await _userRepository.AddAsync(user, cancellationToken);

            return _mapper.Map<CreateUserResult>(user);
        }
    }
}
