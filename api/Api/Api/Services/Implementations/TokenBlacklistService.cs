using Api.Data;
using Api.Models;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Implementations
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly DataContext _context;

        public TokenBlacklistService(DataContext context)
        {
            _context = context;
        }

        public async Task BlacklistTokenAsync(string token, DateTime expiresAt)
        {
            var blacklistedToken = new BlacklistedToken
            {
                Token = token,
                ExpiresAt = expiresAt,
                BlacklistedAt = DateTime.UtcNow
            };

            _context.BlacklistedTokens.Add(blacklistedToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _context.BlacklistedTokens
                .AnyAsync(bt => bt.Token == token && bt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _context.BlacklistedTokens
                .Where(bt => bt.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            _context.BlacklistedTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
