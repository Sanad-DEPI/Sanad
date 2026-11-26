using SANAD.Domain.Constants;

namespace SANAD.Application.Features.Auth;

/// <summary>
/// Application use case: authenticate credentials and issue a token.
/// No Identity or HTTP types are used here.
/// </summary>
public sealed class AuthService(
    IIdentityService identity,
    IJwtTokenGenerator tokens,
    IRefreshTokenStore refreshTokens) : IAuthService
{
    public async Task<AuthLoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return AuthLoginResult.Failure("Email and password are required.");
        }

        var result = await identity.AuthenticateAsync(request.Email.Trim(), request.Password, cancellationToken);
        if (!result.Succeeded || result.UserId is null || result.Email is null)
        {
            return AuthLoginResult.Failure(result.Error ?? "Invalid email or password.");
        }

        var token = tokens.Generate(result.UserId, result.Email, result.Roles);
        var refreshToken = await refreshTokens.CreateAsync(result.UserId, cancellationToken);
        var response = new LoginResponse(
            token.Value,
            token.ExpiresAtUtc,
            refreshToken.Value,
            refreshToken.ExpiresAtUtc,
            result.Email,
            result.Roles);
        return AuthLoginResult.Success(response);
    }

    public async Task<AuthRegistrationResult> RegisterAsync(
        RegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            !AppRoles.PublicRegistration.Contains(request.AccountType, StringComparer.Ordinal))
        {
            return AuthRegistrationResult.Failure("Invalid registration details.");
        }

        var email = request.Email.Trim();
        var created = await identity.RegisterAsync(
            email,
            request.Password,
            request.AccountType,
            cancellationToken);
        return created
            ? AuthRegistrationResult.Success(new RegistrationResponse(
                email,
                request.AccountType,
                request.AccountType == AppRoles.Donor))
            : AuthRegistrationResult.Failure("Registration could not be completed.");
    }

    public async Task<AuthLoginResult> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return AuthLoginResult.Failure("Invalid refresh token.");
        }

        var rotation = await refreshTokens.RotateAsync(request.RefreshToken, cancellationToken);
        if (rotation is null)
        {
            return AuthLoginResult.Failure("Invalid refresh token.");
        }

        var account = await identity.GetAccountAsync(rotation.UserId, cancellationToken);
        if (!account.Succeeded || account.UserId is null || account.Email is null)
        {
            await refreshTokens.RevokeAsync(rotation.RefreshToken, cancellationToken);
            return AuthLoginResult.Failure("Invalid refresh token.");
        }

        var token = tokens.Generate(account.UserId, account.Email, account.Roles);
        var response = new LoginResponse(
            token.Value,
            token.ExpiresAtUtc,
            rotation.RefreshToken,
            rotation.ExpiresAtUtc,
            account.Email,
            account.Roles);
        return AuthLoginResult.Success(response);
    }

    public Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default) =>
        string.IsNullOrWhiteSpace(request.RefreshToken)
            ? Task.CompletedTask
            : refreshTokens.RevokeAsync(request.RefreshToken, cancellationToken);
}
