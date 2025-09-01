using System;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser
{
    /// <summary>
    /// Result DTO returned after creating a user.
    /// </summary>
    public class CreateUserResult
    {
        public Guid Id { get; set; }

        // Mantemos como Username aqui (na WebApi mapeamos para Name)
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }

        // Em string para facilitar integração entre camadas (WebApi faz parse p/ enum)
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
