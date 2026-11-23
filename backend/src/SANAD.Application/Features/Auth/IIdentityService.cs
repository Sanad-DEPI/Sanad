namespace SANAD.Application.Features.Auth;

public interface IIdentityService
{
    Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}
