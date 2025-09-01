using MediatR;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Users.SearchUsers
{
    public class SearchUsersCommand : IRequest<SearchUsersResult>
    {
        // Paginação
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Filtros
        public string? Q { get; set; }        // busca em Username/Email (contains)
        public string? Email { get; set; }    // match exato
        public UserStatus? Status { get; set; }
        public UserRole? Role { get; set; }

        // Ordenação: "username", "-createdAt", "email", "status", "role"
        public string? Sort { get; set; }
    }
}
