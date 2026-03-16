using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken?> GetValidTokenAsync(string token);
        Task CreateTokenAsync(PasswordResetToken resetToken);
        Task MarkTokenAsUsedAsync(int tokenId);
        Task DeleteExpiredTokensAsync();
    }
}
