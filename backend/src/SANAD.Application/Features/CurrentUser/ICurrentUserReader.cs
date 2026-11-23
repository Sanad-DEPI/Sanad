namespace SANAD.Application.Features.CurrentUser;

public interface ICurrentUserReader
{
    Task<SANAD.Domain.Models.CurrentUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
}
