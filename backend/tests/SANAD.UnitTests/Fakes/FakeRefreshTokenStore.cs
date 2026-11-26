using SANAD.Application.Features.Auth;

namespace SANAD.UnitTests.Fakes;

internal sealed class FakeRefreshTokenStore(
    TokenResult? issue = null,
    RefreshTokenRotation? rotation = null) : IRefreshTokenStore
{
    public int CreateCallCount { get; private set; }
    public int RotateCallCount { get; private set; }
    public int RevokeCallCount { get; private set; }
    public string? CapturedRefreshToken { get; private set; }

    public Task<TokenResult> CreateAsync(string userId, CancellationToken cancellationToken = default)
    {
        CreateCallCount++;
        return Task.FromResult(issue ?? new TokenResult("refresh-token", DateTime.UtcNow.AddDays(7)));
    }

    public Task<RefreshTokenRotation?> RotateAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        RotateCallCount++;
        CapturedRefreshToken = refreshToken;
        return Task.FromResult(rotation);
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        RevokeCallCount++;
        CapturedRefreshToken = refreshToken;
        return Task.CompletedTask;
    }

    public Task<int> DeleteExpiredAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(0);
}
