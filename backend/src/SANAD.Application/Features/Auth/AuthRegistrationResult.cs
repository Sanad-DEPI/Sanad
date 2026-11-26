namespace SANAD.Application.Features.Auth;

public sealed record AuthRegistrationResult(
    bool Succeeded,
    RegistrationResponse? Response,
    string? Error)
{
    public static AuthRegistrationResult Success(RegistrationResponse response) =>
        new(true, response, null);

    public static AuthRegistrationResult Failure(string error) =>
        new(false, null, error);
}
