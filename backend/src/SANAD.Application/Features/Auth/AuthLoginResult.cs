namespace SANAD.Application.Features.Auth;

public sealed record AuthLoginResult(bool Succeeded, LoginResponse? Response, string? Error)
{
    public static AuthLoginResult Success(LoginResponse response) => new(true, response, null);
    public static AuthLoginResult Failure(string error) => new(false, null, error);
}
