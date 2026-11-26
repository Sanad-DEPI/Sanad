namespace SANAD.Application.Features.Auth;

public interface IRefreshTokenStore
{
    Task<TokenResult> CreateAsync(string userId, CancellationToken cancellationToken = default);

    Task<RefreshTokenRotation?> RotateAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<int> DeleteExpiredAsync(CancellationToken cancellationToken = default);
}
