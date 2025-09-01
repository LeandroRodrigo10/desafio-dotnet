using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        // Novo: busca paginada com filtros
        Task<(IReadOnlyList<User> Items, int Total)> SearchAsync(
            int page,
            int pageSize,
            string? q = null,
            string? email = null,
            UserStatus? status = null,
            UserRole? role = null,
            string? sort = null,
            CancellationToken cancellationToken = default
        );
    }
}
