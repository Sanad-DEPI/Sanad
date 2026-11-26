using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SANAD.Application.Features.Auth;
using SANAD.Infrastructure.Configuration;
using SANAD.Infrastructure.Persistence;

namespace SANAD.Infrastructure.Identity;

public sealed class IdentityRefreshTokenStore(
    AppDbContext dbContext,
    IOptions<RefreshTokenOptions> options) : IRefreshTokenStore
{
    private const string LoginProvider = "SANAD.RefreshToken";
    private const string ExpiryFormat = "O";

    public async Task<TokenResult> CreateAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var token = GenerateToken();
        var expiresAtUtc = DateTime.UtcNow.AddDays(options.Value.LifetimeDays);
        dbContext.UserTokens.Add(new IdentityUserToken<string>
        {
            UserId = userId,
            LoginProvider = LoginProvider,
            Name = Hash(token),
            Value = expiresAtUtc.ToString(ExpiryFormat, CultureInfo.InvariantCulture)
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return new TokenResult(token, expiresAtUtc);
    }

    public async Task<RefreshTokenRotation?> RotateAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = Hash(refreshToken);
        var storedToken = await dbContext.UserTokens.SingleOrDefaultAsync(
            token => token.LoginProvider == LoginProvider && token.Name == tokenHash,
            cancellationToken);
        if (storedToken is null)
        {
            return null;
        }

        dbContext.UserTokens.Remove(storedToken);
        if (!DateTime.TryParseExact(
                storedToken.Value,
                ExpiryFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var expiresAtUtc) ||
            expiresAtUtc <= DateTime.UtcNow)
        {
            await SaveConsumptionAsync(cancellationToken);
            return null;
        }

        var replacement = GenerateToken();
        var replacementExpiresAtUtc = DateTime.UtcNow.AddDays(options.Value.LifetimeDays);
        dbContext.UserTokens.Add(new IdentityUserToken<string>
        {
            UserId = storedToken.UserId,
            LoginProvider = LoginProvider,
            Name = Hash(replacement),
            Value = replacementExpiresAtUtc.ToString(ExpiryFormat, CultureInfo.InvariantCulture)
        });

        if (!await SaveConsumptionAsync(cancellationToken))
        {
            return null;
        }

        return new RefreshTokenRotation(storedToken.UserId, replacement, replacementExpiresAtUtc);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = Hash(refreshToken);
        var storedToken = await dbContext.UserTokens.SingleOrDefaultAsync(
            token => token.LoginProvider == LoginProvider && token.Name == tokenHash,
            cancellationToken);
        if (storedToken is null)
        {
            return;
        }

        dbContext.UserTokens.Remove(storedToken);
        await SaveConsumptionAsync(cancellationToken);
    }

    private async Task<bool> SaveConsumptionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    private static string GenerateToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
