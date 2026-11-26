namespace SANAD.Application.Features.Auth;

public sealed record RefreshTokenRotation(
    string UserId,
    string RefreshToken,
    DateTime ExpiresAtUtc);
