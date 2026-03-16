using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly DataContext _context;

        public PasswordResetTokenRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .Include(prt => prt.User)
                .FirstOrDefaultAsync(prt => 
                    prt.Token == token && 
                    !prt.IsUsed && 
                    prt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task CreateTokenAsync(PasswordResetToken resetToken)
        {
            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();
        }

        public async Task MarkTokenAsUsedAsync(int tokenId)
        {
            var token = await _context.PasswordResetTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var expiredTokens = await _context.PasswordResetTokens
                .Where(prt => prt.ExpiresAt <= DateTime.UtcNow || prt.IsUsed)
                .ToListAsync();

            _context.PasswordResetTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
