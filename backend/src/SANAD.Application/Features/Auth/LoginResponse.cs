namespace SANAD.Application.Features.Auth;

public sealed record LoginResponse(
    string Token,
    DateTime ExpiresAtUtc,
    string Email,
    IReadOnlyList<string> Roles);
