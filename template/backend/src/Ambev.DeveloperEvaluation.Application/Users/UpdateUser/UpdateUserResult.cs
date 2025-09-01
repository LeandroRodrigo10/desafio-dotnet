using System;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser
{
    /// <summary>
    /// Result returned after updating a user
    /// </summary>
    public class UpdateUserResult
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
