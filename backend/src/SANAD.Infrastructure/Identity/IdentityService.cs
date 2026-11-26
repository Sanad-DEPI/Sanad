using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SANAD.Application.Features.Auth;
using SANAD.Domain.Constants;

namespace SANAD.Infrastructure.Identity;

/// <summary>
/// Infrastructure implementation of the application's authentication abstraction.
/// </summary>
public sealed class IdentityService(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signInManager,
    ILogger<IdentityService> logger) : IIdentityService
{
    public async Task<AuthResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            return AuthResult.Failure("Invalid email or password.");
        }

        var signIn = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!signIn.Succeeded)
        {
            return AuthResult.Failure("Invalid email or password.");
        }

        var roles = await users.GetRolesAsync(user);
        if (!HasValidRoles(roles) ||
            (roles.Contains(AppRoles.Donor, StringComparer.Ordinal) && !user.EmailConfirmed))
        {
            return AuthResult.Failure("Invalid email or password.");
        }

        return AuthResult.Success(user.Id, user.Email ?? email, roles.ToArray());
    }

    public async Task<AuthResult> GetAccountAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await users.FindByIdAsync(userId);
        if (user is null || await users.IsLockedOutAsync(user))
        {
            return AuthResult.Failure("Invalid account.");
        }

        var roles = await users.GetRolesAsync(user);
        if (!HasValidRoles(roles) ||
            (roles.Contains(AppRoles.Donor, StringComparer.Ordinal) && !user.EmailConfirmed))
        {
            return AuthResult.Failure("Invalid account.");
        }

        return AuthResult.Success(user.Id, user.Email ?? string.Empty, roles.ToArray());
    }

    public async Task<bool> RegisterAsync(
        string email,
        string password,
        string publicRole,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!AppRoles.PublicRegistration.Contains(publicRole, StringComparer.Ordinal))
        {
            return false;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = false
        };

        var created = await users.CreateAsync(user, password);
        if (!created.Succeeded)
        {
            return false;
        }

        var assigned = await users.AddToRoleAsync(user, publicRole);
        if (assigned.Succeeded)
        {
            return true;
        }

        logger.LogError(
            "Public account role assignment failed. Identity error codes: {ErrorCodes}",
            string.Join(", ", assigned.Errors.Select(error => error.Code)));

        var deleted = await users.DeleteAsync(user);
        if (!deleted.Succeeded)
        {
            logger.LogCritical(
                "Failed to clean up an incomplete public account. Identity error codes: {ErrorCodes}",
                string.Join(", ", deleted.Errors.Select(error => error.Code)));
            throw new InvalidOperationException("Incomplete public account could not be removed.");
        }

        return false;
    }

    private static bool HasValidRoles(IList<string> roles)
    {
        var publicRoleCount = roles.Count(role =>
            AppRoles.PublicRegistration.Contains(role, StringComparer.Ordinal));

        return roles.Count > 0 &&
               roles.All(role => AppRoles.All.Contains(role, StringComparer.Ordinal)) &&
               publicRoleCount <= 1 &&
               (publicRoleCount == 0 || roles.Count == 1);
    }
}
