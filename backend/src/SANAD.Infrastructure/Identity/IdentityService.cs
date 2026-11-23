using Microsoft.AspNetCore.Identity;
using SANAD.Application.Features.Auth;

namespace SANAD.Infrastructure.Identity;

/// <summary>
/// Infrastructure implementation of the application's authentication abstraction.
/// </summary>
public sealed class IdentityService(UserManager<ApplicationUser> users) : IIdentityService
{
    public async Task<AuthResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await users.FindByEmailAsync(email);
        if (user is null || !await users.CheckPasswordAsync(user, password))
        {
            return AuthResult.Failure("Invalid email or password.");
        }

        var roles = await users.GetRolesAsync(user);
        return AuthResult.Success(user.Id, user.Email ?? email, roles.ToArray());
    }
}
