using SANAD.Application.Features.Auth;

namespace SANAD.UnitTests.Fakes;

internal sealed class FakeIdentityService : IIdentityService
{
    private readonly AuthResult _result;

    public FakeIdentityService(
        AuthResult result,
        bool registrationSucceeded = true,
        AuthResult? accountResult = null)
    {
        _result = result;
        RegistrationSucceeded = registrationSucceeded;
        AccountResult = accountResult ?? result;
    }

    public int CallCount { get; private set; }
    public string? CapturedEmail { get; private set; }
    public string? CapturedPassword { get; private set; }
    public string? CapturedPublicRole { get; private set; }
    public bool RegistrationSucceeded { get; }
    public AuthResult AccountResult { get; }

    public Task<AuthResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        CallCount++;
        CapturedEmail = email;
        CapturedPassword = password;
        return Task.FromResult(_result);
    }

    public Task<AuthResult> GetAccountAsync(string userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(AccountResult);

    public Task<bool> RegisterAsync(
        string email,
        string password,
        string publicRole,
        CancellationToken cancellationToken = default)
    {
        CapturedEmail = email;
        CapturedPassword = password;
        CapturedPublicRole = publicRole;
        return Task.FromResult(RegistrationSucceeded);
    }
}
