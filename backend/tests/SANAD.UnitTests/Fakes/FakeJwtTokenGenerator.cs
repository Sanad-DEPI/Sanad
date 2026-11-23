using SANAD.Application.Features.Auth;

namespace SANAD.UnitTests.Fakes;

internal sealed class FakeJwtTokenGenerator(TokenResult token) : IJwtTokenGenerator
{
    public int CallCount { get; private set; }
    public string? CapturedUserId { get; private set; }
    public IReadOnlyList<string>? CapturedRoles { get; private set; }

    public TokenResult Generate(string userId, string email, IReadOnlyList<string> roles)
    {
        CallCount++;
        CapturedUserId = userId;
        CapturedRoles = roles;
        return token;
    }
}
