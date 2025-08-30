using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface ISaleRepository
    {
        Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNumberAsync(string number, CancellationToken cancellationToken = default);

        Task AddAsync(Sale sale, CancellationToken cancellationToken = default);
        Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search sales with pagination and optional filters/sorting.
        /// sort example: "date", "-date", "customer", "-createdAt"
        /// </summary>
        Task<(IReadOnlyList<Sale> Items, int Total)> SearchAsync(
            int page,
            int pageSize,
            string? q = null,
            string? customer = null,
            string? branch = null,
            SaleStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? sort = null,
            CancellationToken cancellationToken = default);
    }
}
