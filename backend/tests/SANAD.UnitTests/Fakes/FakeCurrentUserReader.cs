using SANAD.Application.Features.CurrentUser;

namespace SANAD.UnitTests.Fakes;

internal sealed class FakeCurrentUserReader(SANAD.Domain.Models.CurrentUser? user) : ICurrentUserReader
{
    public Task<SANAD.Domain.Models.CurrentUser?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default) => Task.FromResult(user);
}
