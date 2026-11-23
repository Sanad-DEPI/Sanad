namespace SANAD.Application.Features.Auth;

/// <summary>
/// Outcome of checking credentials, independent of ASP.NET Core Identity.
/// </summary>
public sealed record AuthResult(
    bool Succeeded,
    string? UserId,
    string? Email,
    IReadOnlyList<string> Roles,
    string? Error)
{
    public static AuthResult Success(string userId, string email, IReadOnlyList<string> roles) =>
        new(true, userId, email, roles, null);

    public static AuthResult Failure(string error) => new(false, null, null, [], error);
}
