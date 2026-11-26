using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SANAD.Domain.Constants;

namespace SANAD.Infrastructure.Identity;

public sealed record SuperAdminSeedOptions(string Email, string Password);

/// <summary>
/// Optional development-only manager credentials. Never supply these in production.
/// </summary>
public sealed record ManagerSeedOptions(string Email, string Password);

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        SuperAdminSeedOptions superAdmin,
        ManagerSeedOptions? manager,
        ILogger logger)
    {
        ValidateCredentials(superAdmin.Email, superAdmin.Password, "Super Admin");
        if (manager is not null)
        {
            ValidateCredentials(manager.Email, manager.Password, "Manager");
        }

        foreach (var role in AppRoles.All)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var created = await roleManager.CreateAsync(new IdentityRole(role));
            if (!created.Succeeded && !await roleManager.RoleExistsAsync(role))
            {
                LogIdentityFailure(logger, "Role creation failed", created);
                throw new InvalidOperationException($"Required role '{role}' could not be created.");
            }
        }

        await EnsureAccountAsync(
            userManager,
            superAdmin.Email,
            superAdmin.Password,
            AppRoles.SuperAdmin,
            "Super Admin",
            logger);

        if (manager is not null)
        {
            await EnsureAccountAsync(
                userManager,
                manager.Email,
                manager.Password,
                AppRoles.Manager,
                "Manager",
                logger);
        }
    }

    private static void ValidateCredentials(string email, string password, string accountName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Initial {accountName} credentials are required. Configure them using user secrets or secure environment variables.");
        }
    }

    private static async Task EnsureAccountAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role,
        string accountName,
        ILogger logger)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var created = await userManager.CreateAsync(user, password);
            if (!created.Succeeded)
            {
                user = await userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    LogIdentityFailure(logger, $"{accountName} provisioning failed", created);
                    throw new InvalidOperationException($"Initial {accountName} account could not be provisioned.");
                }
            }
        }

        var existingRoles = await userManager.GetRolesAsync(user);
        if (existingRoles.Any(existingRole =>
                AppRoles.PublicRegistration.Contains(existingRole, StringComparer.Ordinal)))
        {
            logger.LogError(
                "Initial {AccountName} account already has a public role. Provisioning was stopped.",
                accountName);
            throw new InvalidOperationException($"Initial {accountName} account has incompatible roles.");
        }

        if (existingRoles.Contains(role, StringComparer.Ordinal))
        {
            return;
        }

        if (existingRoles.Count != 0)
        {
            logger.LogError(
                "Initial {AccountName} account has incompatible assigned roles. Provisioning was stopped.",
                accountName);
            throw new InvalidOperationException($"Initial {accountName} account has incompatible roles.");
        }

        var assigned = await userManager.AddToRoleAsync(user, role);
        if (!assigned.Succeeded && !await userManager.IsInRoleAsync(user, role))
        {
            LogIdentityFailure(logger, $"{accountName} role assignment failed", assigned);
            throw new InvalidOperationException($"Initial {accountName} role could not be assigned.");
        }
    }

    private static void LogIdentityFailure(
        ILogger logger,
        string operation,
        IdentityResult result) =>
        logger.LogError(
            "{Operation}. Identity error codes: {ErrorCodes}",
            operation,
            string.Join(", ", result.Errors.Select(error => error.Code)));
}
