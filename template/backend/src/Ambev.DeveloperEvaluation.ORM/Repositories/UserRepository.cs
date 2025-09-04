#nullable enable
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
        private readonly DefaultContext _db;
        private readonly IMemoryCache? _cache;

        public UserRepository(DefaultContext db, IMemoryCache? cache = null)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _cache = cache;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var cacheKey = $"user:{id}";
            if (_cache != null && _cache.TryGetValue(cacheKey, out object? obj) && obj is User cached)
                return cached;

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user != null && _cache != null)
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));

            return user;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var key = email.Trim().ToLowerInvariant();
            var cacheKey = $"user:email:{key}";

            if (_cache != null && _cache.TryGetValue(cacheKey, out object? obj) && obj is User cached)
                return cached;

            // 🔎 Debug temporário para entender o cenário nos testes
            try
            {
                var total = await _db.Users.CountAsync(ct);
                Console.WriteLine($"[REPO][GetByEmailAsync] Procurando por '{key}' (total users: {total})");
            }
            catch
            {
                // ignore qualquer falha de log
            }

            // ✅ Comparação robusta: TRIM + ToLower no lado do BD também
            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.Email != null &&
                    u.Email.Trim().ToLower() == key, ct);

            if (user == null)
            {
                // Tentativa extra (útil se o provedor/banco tratar casing de forma estranha)
                user = await _db.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email, ct);
            }

            if (user != null && _cache != null)
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));

            Console.WriteLine($"[REPO][GetByEmailAsync] Encontrou? {(user != null ? "SIM" : "NÃO")} -> '{user?.Email}'");

            return user;
        }

        // Assinatura correta: método não retorna valor
        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);

            _cache?.Remove($"user:{user.Id}");
            if (!string.IsNullOrWhiteSpace(user.Email))
                _cache?.Remove($"user:email:{user.Email.Trim().ToLowerInvariant()}");
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);

            _cache?.Remove($"user:{user.Id}");
            if (!string.IsNullOrWhiteSpace(user.Email))
                _cache?.Remove($"user:email:{user.Email.Trim().ToLowerInvariant()}");
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (entity == null) return;

            _db.Users.Remove(entity);
            await _db.SaveChangesAsync(ct);

            _cache?.Remove($"user:{id}");
            if (!string.IsNullOrWhiteSpace(entity.Email))
                _cache?.Remove($"user:email:{entity.Email.Trim().ToLowerInvariant()}");
        }

        public async Task<(IReadOnlyList<User> Items, int Total)> SearchAsync(
            int page,
            int pageSize,
            string? q = null,
            string? email = null,
            UserStatus? status = null,
            UserRole? role = null,
            string? sort = null,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _db.Users.AsNoTracking().AsQueryable();

            // busca livre em Username/Email/Phone
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(u =>
                    EF.Functions.ILike(u.Username, $"%{term}%") ||
                    EF.Functions.ILike(u.Email, $"%{term}%") ||
                    (u.Phone != null && u.Phone.Contains(term)));
            }

            // filtro por email
            if (!string.IsNullOrWhiteSpace(email))
            {
                var e = email.Trim().ToLowerInvariant();
                query = query.Where(u =>
                    u.Email.ToLower() == e || EF.Functions.ILike(u.Email, $"%{e}%"));
            }

            if (status.HasValue)
                query = query.Where(u => u.Status == status.Value);

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            // ordenação
            query = (sort?.ToLowerInvariant()) switch
            {
                "email" => query.OrderBy(u => u.Email).ThenBy(u => u.Username),
                "createdat" => query.OrderBy(u => u.CreatedAt),
                "updatedat" => query.OrderBy(u => u.UpdatedAt),
                "role" => query.OrderBy(u => u.Role).ThenBy(u => u.Username),
                "status" => query.OrderBy(u => u.Status).ThenBy(u => u.Username),
                _ => query.OrderBy(u => u.Username)
            };

            var total = await query.CountAsync(ct);
            var skip = (page - 1) * pageSize;
            var items = await query.Skip(skip).Take(pageSize).ToListAsync(ct);

            return (items, total);
        }
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

}
