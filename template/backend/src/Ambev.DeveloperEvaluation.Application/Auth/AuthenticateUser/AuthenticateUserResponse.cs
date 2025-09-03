namespace Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser
{
    public sealed class AuthenticateUserResponse
    {
        public string Token { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
