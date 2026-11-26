namespace SANAD.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthLoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthRegistrationResult> RegisterAsync(
        RegistrationRequest request,
        CancellationToken cancellationToken = default);

    Task<AuthLoginResult> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}
