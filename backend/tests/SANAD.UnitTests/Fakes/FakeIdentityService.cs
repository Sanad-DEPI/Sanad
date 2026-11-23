using SANAD.Application.Features.Auth;

namespace SANAD.UnitTests.Fakes;

internal sealed class FakeIdentityService(AuthResult result) : IIdentityService
{
    public int CallCount { get; private set; }
    public string? CapturedEmail { get; private set; }
    public string? CapturedPassword { get; private set; }

    public Task<AuthResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        CallCount++;
        CapturedEmail = email;
        CapturedPassword = password;
        return Task.FromResult(result);
    }
}
