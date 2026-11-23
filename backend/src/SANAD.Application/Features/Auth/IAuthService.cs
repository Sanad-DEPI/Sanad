namespace SANAD.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthLoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
