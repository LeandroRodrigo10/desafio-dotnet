using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Users.SearchUsers
{
    public class SearchUsersResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<UserSummary> Items { get; set; } = new();

        public class UserSummary
        {
            public Guid Id { get; set; }
            public string Username { get; set; } = default!;
            public string Email { get; set; } = default!;
            public string? Phone { get; set; }
            public string Status { get; set; } = default!;
            public string Role { get; set; } = default!;
            public DateTime CreatedAt { get; set; }
        }
    }
}
