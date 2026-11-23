using SANAD.Application.Features.CurrentUser;
using SANAD.UnitTests.Fakes;

namespace SANAD.UnitTests.Features.CurrentUser;

public sealed class CurrentUserServiceTests
{
    [Fact]
    public async Task GetAsync_WhenUserExists_MapsDomainModelToDto()
    {
        var domainUser = new SANAD.Domain.Models.CurrentUser(
            "user-1",
            "user@example.test",
            ["Manager", "Donor"]);
        var service = new CurrentUserService(new FakeCurrentUserReader(domainUser));

        var result = await service.GetAsync("user-1");

        Assert.NotNull(result);
        Assert.Equal(domainUser.Id, result.Id);
        Assert.Equal(domainUser.Email, result.Email);
        Assert.Equal(domainUser.Roles, result.Roles);
    }

    [Fact]
    public async Task GetAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var service = new CurrentUserService(new FakeCurrentUserReader(null));

        var result = await service.GetAsync("missing-user");

        Assert.Null(result);
    }
}
