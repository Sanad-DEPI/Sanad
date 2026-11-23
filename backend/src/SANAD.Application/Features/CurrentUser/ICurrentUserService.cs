namespace SANAD.Application.Features.CurrentUser;

public interface ICurrentUserService
{
    Task<CurrentUserDto?> GetAsync(string userId, CancellationToken cancellationToken = default);
}
