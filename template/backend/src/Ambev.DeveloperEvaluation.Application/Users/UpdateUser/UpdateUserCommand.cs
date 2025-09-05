using System;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser
{
    public class UpdateUserCommand : IRequest<UpdateUserResult>
    {
        public Guid Id { get; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }

        // Opcional: só atualiza se vier preenchida
        public string? Password { get; set; }

        public UpdateUserCommand(Guid id)
        {
            Id = id;
        }
    }
}
