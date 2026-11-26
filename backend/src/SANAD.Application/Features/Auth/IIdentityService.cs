namespace SANAD.Application.Features.Auth;

public interface IIdentityService
{
    Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<AuthResult> GetAccountAsync(string userId, CancellationToken cancellationToken = default);

    Task<bool> RegisterAsync(
        string email,
        string password,
        string publicRole,
        CancellationToken cancellationToken = default);
}
