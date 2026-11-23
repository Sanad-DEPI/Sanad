namespace SANAD.Application.Features.Auth;

/// <summary>
/// Application use case: authenticate credentials and issue a token.
/// No Identity or HTTP types are used here.
/// </summary>
public sealed class AuthService(IIdentityService identity, IJwtTokenGenerator tokens) : IAuthService
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
        var response = new LoginResponse(token.Value, token.ExpiresAtUtc, result.Email, result.Roles);
        return AuthLoginResult.Success(response);
    }
}
