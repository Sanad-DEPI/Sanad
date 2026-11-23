using Microsoft.AspNetCore.Identity;
using SANAD.Application.Features.CurrentUser;

namespace SANAD.Infrastructure.Identity;

/// <summary>
/// Infrastructure implementation of the reference feature's reader abstraction.
/// </summary>
public sealed class CurrentUserReader(UserManager<ApplicationUser> users) : ICurrentUserReader
{
    public async Task<SANAD.Domain.Models.CurrentUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await users.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        var roles = await users.GetRolesAsync(user);
        return new SANAD.Domain.Models.CurrentUser(user.Id, user.Email ?? string.Empty, roles.ToArray());
    }
}
