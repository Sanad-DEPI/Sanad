using Microsoft.AspNetCore.Identity;
using SANAD.Domain.Constants;

namespace SANAD.Infrastructure.Identity;

/// <summary>
/// Optional development-only manager credentials. Never supply these in production.
/// </summary>
public sealed record ManagerSeedOptions(string Email, string Password);

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ManagerSeedOptions? manager = null)
    {
        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (manager is null || string.IsNullOrWhiteSpace(manager.Email) || string.IsNullOrWhiteSpace(manager.Password))
        {
            return;
        }

        var user = await userManager.FindByEmailAsync(manager.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = manager.Email,
                Email = manager.Email,
                EmailConfirmed = true
            };

            var created = await userManager.CreateAsync(user, manager.Password);
            if (!created.Succeeded)
            {
                throw new InvalidOperationException(
                    "Initial Manager account could not be created: " +
                    string.Join("; ", created.Errors.Select(error => error.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, AppRoles.Manager))
        {
            await userManager.AddToRoleAsync(user, AppRoles.Manager);
        }
    }
}
