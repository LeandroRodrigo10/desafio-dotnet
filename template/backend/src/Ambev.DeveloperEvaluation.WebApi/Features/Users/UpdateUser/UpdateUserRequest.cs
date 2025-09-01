using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser
{
    public class UpdateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
        public string? Password { get; set; }
    }
}
