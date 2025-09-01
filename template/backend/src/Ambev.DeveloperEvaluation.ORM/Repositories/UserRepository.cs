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
    public class UserRepository : IUserRepository
    {
        private readonly DefaultContext _context;

        public UserRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Set<User>().AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Set<User>().Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (entity is null) return;

            _context.Set<User>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<User> Items, int Total)> SearchAsync(
            int page,
            int pageSize,
            string? q = null,
            string? email = null,
            UserStatus? status = null,
            UserRole? role = null,
            string? sort = null,
            CancellationToken cancellationToken = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            IQueryable<User> query = _context.Set<User>().AsNoTracking();

            // Busca textual em Username/Email (contains)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim();
                query = query.Where(u => u.Username.Contains(t) || u.Email.Contains(t));
            }

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email == email);

            if (status.HasValue)
                query = query.Where(u => u.Status == status.Value);

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            var total = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, sort);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        private static IQueryable<User> ApplySorting(IQueryable<User> query, string? sort)
        {
            // Padrão: Username asc, depois Email
            if (string.IsNullOrWhiteSpace(sort))
                return query.OrderBy(u => u.Username).ThenBy(u => u.Email);

            var s = sort.Trim();
            var desc = s.StartsWith("-");
            var key = desc ? s[1..] : s;

            return key.ToLowerInvariant() switch
            {
                "username" => desc ? query.OrderByDescending(x => x.Username) : query.OrderBy(x => x.Username),
                "email" => desc ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
                "status" => desc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
                "role" => desc ? query.OrderByDescending(x => x.Role) : query.OrderBy(x => x.Role),
                "createdat" => desc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                _ => query.OrderBy(x => x.Username).ThenBy(x => x.Email)
            };
        }
    }
}
