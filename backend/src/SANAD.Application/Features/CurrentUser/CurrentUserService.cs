namespace SANAD.Application.Features.CurrentUser;

/// <summary>
/// Reference application service showing the agreed layer flow for new backend features.
/// </summary>
public sealed class CurrentUserService(ICurrentUserReader reader) : ICurrentUserService
{
    public async Task<CurrentUserDto?> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await reader.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        return new CurrentUserDto(user.Id, user.Email, user.Roles);
    }
}
