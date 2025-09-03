using System;

namespace Ambev.DeveloperEvaluation.Common.Security
{
    public sealed class BCryptPasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 10;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return string.Empty;

            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }

        public bool Verify(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
