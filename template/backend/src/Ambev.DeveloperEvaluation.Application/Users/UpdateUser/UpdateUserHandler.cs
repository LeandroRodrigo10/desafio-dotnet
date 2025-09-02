using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Security;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser
{
    /// <summary>
    /// Handles the update of an existing user, applying validation and
    /// ensuring that the email remains unique in the system. Optional fields
    /// like username, phone and password are updated only when provided.
    /// </summary>
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateUserHandler(
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Validate the incoming request using the validator. This helps
            // surface format issues before hitting the domain layer.
            var validator = new UpdateUserValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Retrieve the existing user by id
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                throw new InvalidOperationException($"User with Id {request.Id} not found");

            // Normalise incoming email and username (trim and lower as needed)
            var trimmedEmail = request.Email?.Trim().ToLowerInvariant();
            var trimmedUsername = request.Username?.Trim();

            // Update the email if it's different and ensure no other user has the same email
            if (!string.IsNullOrWhiteSpace(trimmedEmail) &&
                !string.Equals(user.Email, trimmedEmail, StringComparison.OrdinalIgnoreCase))
            {
                var otherUser = await _userRepository.GetByEmailAsync(trimmedEmail, cancellationToken);
                if (otherUser != null && otherUser.Id != user.Id)
                    throw new InvalidOperationException($"Email '{trimmedEmail}' is already in use by another user");

                user.Email = trimmedEmail!;
            }

            // Update the username if supplied and different
            if (!string.IsNullOrWhiteSpace(trimmedUsername) &&
                !string.Equals(user.Username, trimmedUsername, StringComparison.Ordinal))
            {
                user.Username = trimmedUsername!;
            }

            // Update phone number if provided
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                user.Phone = request.Phone.Trim();
            }

            // Hash and update the password if provided
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.Password = _passwordHasher.HashPassword(request.Password);
            }

            // Always update role and status
            user.Role = request.Role;
            user.Status = request.Status;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            // Map the updated domain user back to the result DTO
            return _mapper.Map<UpdateUserResult>(user);
        }
    }
}
