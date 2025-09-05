using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly DefaultContext _context;
        private readonly IMemoryCache? _cache;

        public UserRepository(DefaultContext context, IMemoryCache? cache = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user:{id}";
            if (_cache != null && _cache.TryGetValue(cacheKey, out object? obj) && obj is User cached)
                return cached;

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user != null && _cache != null)
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));

            return user;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var key = email.Trim().ToLowerInvariant();
            var cacheKey = $"user:email:{key}";

            if (_cache != null && _cache.TryGetValue(cacheKey, out object? obj) && obj is User cached)
                return cached;

            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.Trim().ToLower() == key, cancellationToken)
                ?? await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user != null && _cache != null)
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));

            return user;
        }

        // Interface: Task AddAsync(User, CancellationToken)
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            InvalidateCache(user);
        }

        // Interface: Task UpdateAsync(User, CancellationToken)
        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            InvalidateCache(user);
        }

        // Interface: Task DeleteAsync(Guid, CancellationToken)
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (entity == null) return;

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            InvalidateCache(entity);
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
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(u =>
                    EF.Functions.ILike(u.Username, $"%{term}%") ||
                    EF.Functions.ILike(u.Email, $"%{term}%") ||
                    (u.Phone != null && u.Phone.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var e = email.Trim().ToLowerInvariant();
                query = query.Where(u => u.Email.ToLower() == e || EF.Functions.ILike(u.Email, $"%{e}%"));
            }

            if (status.HasValue)
                query = query.Where(u => u.Status == status.Value);

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            query = (sort?.ToLowerInvariant()) switch
            {
                "email" => query.OrderBy(u => u.Email).ThenBy(u => u.Username),
                "createdat" => query.OrderBy(u => u.CreatedAt),
                "updatedat" => query.OrderBy(u => u.UpdatedAt),
                "role" => query.OrderBy(u => u.Role).ThenBy(u => u.Username),
                "status" => query.OrderBy(u => u.Status).ThenBy(u => u.Username),
                _ => query.OrderBy(u => u.Username)
            };

            var total = await query.CountAsync(cancellationToken);
            var skip = (page - 1) * pageSize;
            var items = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

            return (items, total);
        }

        private void InvalidateCache(User user)
        {
            _cache?.Remove($"user:{user.Id}");
            if (!string.IsNullOrWhiteSpace(user.Email))
                _cache?.Remove($"user:email:{user.Email.Trim().ToLowerInvariant()}");
        }
    }
}