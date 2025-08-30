using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DefaultContext _context;

        public SaleRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Sale>()
                .AsNoTracking()
                .Include(s => s.Items) // << ajustado
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByNumberAsync(string number, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Sale>()
                .AsNoTracking()
                .AnyAsync(s => s.Number == number, cancellationToken);
        }

        public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await _context.Set<Sale>().AddAsync(sale, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            _context.Set<Sale>().Update(sale);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Sale>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (entity is null) return;

            _context.Set<Sale>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Sale> Items, int Total)> SearchAsync(
            int page,
            int pageSize,
            string? q = null,
            string? customer = null,
            string? branch = null,
            SaleStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? sort = null,
            CancellationToken cancellationToken = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            IQueryable<Sale> query = _context.Set<Sale>().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qTrim = q.Trim();
                query = query.Where(s =>
                    s.Number.Contains(qTrim) ||
                    s.Customer.Contains(qTrim) ||
                    s.Branch.Contains(qTrim));
            }

            if (!string.IsNullOrWhiteSpace(customer))
                query = query.Where(s => s.Customer.Contains(customer.Trim()));

            if (!string.IsNullOrWhiteSpace(branch))
                query = query.Where(s => s.Branch.Contains(branch.Trim()));

            if (status.HasValue)
                query = query.Where(s => s.Status == status.Value);

            if (dateFrom.HasValue)
                query = query.Where(s => s.Date >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(s => s.Date <= dateTo.Value);

            var total = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, sort);

            var items = await query
                .Include(s => s.Items) // << ajustado
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        private static IQueryable<Sale> ApplySorting(IQueryable<Sale> query, string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return query.OrderByDescending(s => s.Date).ThenBy(s => s.Number);

            var s = sort.Trim();
            var desc = s.StartsWith("-");
            var key = desc ? s[1..] : s;

            return (key.ToLowerInvariant()) switch
            {
                "date" => desc ? query.OrderByDescending(x => x.Date) : query.OrderBy(x => x.Date),
                "customer" => desc ? query.OrderByDescending(x => x.Customer) : query.OrderBy(x => x.Customer),
                "branch" => desc ? query.OrderByDescending(x => x.Branch) : query.OrderBy(x => x.Branch),
                "number" => desc ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number),
                "status" => desc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
                "totalamount" => desc ? query.OrderByDescending(x => x.TotalAmount) : query.OrderBy(x => x.TotalAmount),
                _ => query.OrderByDescending(x => x.Date).ThenBy(x => x.Number)
            };
        }
    }
}
