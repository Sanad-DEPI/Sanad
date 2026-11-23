namespace SANAD.Application.Features.Auth;

public sealed record TokenResult(string Value, DateTime ExpiresAtUtc);
