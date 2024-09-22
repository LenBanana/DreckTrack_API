using DreckTrack_API.Database;
using DreckTrack_API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DreckTrack_API.Services;

public class RefreshTokenService(ApplicationDbContext context)
{
    public async Task SaveRefreshTokenAsync(Guid userId, string refreshToken, int durationInDays)
    {
        var token = new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            Expires = DateTime.UtcNow.AddDays(durationInDays),
            Created = DateTime.UtcNow,
            IsRevoked = false
        };
        context.RefreshTokens.Add(token);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        token.Revoked = DateTime.UtcNow;
        context.RefreshTokens.Update(token);
        await context.SaveChangesAsync();
    }
}
