using System.Net;
using SANAD.Application.Common;
using SANAD.Application.Features.CurrentUser;
using SANAD.Domain.Constants;

namespace SANAD.IntegrationTests;

public sealed class CurrentUserEndpointTests(SanadWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Me_WithValidToken_ReturnsCurrentUser()
    {
        var (email, password) = await CreateUserAsync(AppRoles.FieldOfficer, AppRoles.Manager);
        var token = await LoginAsync(email, password);

        var response = await Client.SendAsync(GetWithToken("/api/me", token));
        var body = await ReadApiAsync<CurrentUserDto>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.Succeeded);
        Assert.NotNull(body.Data);
        Assert.Equal(email, body.Data.Email);
        Assert.False(string.IsNullOrWhiteSpace(body.Data.Id));
        Assert.Contains(AppRoles.FieldOfficer, body.Data.Roles);
        Assert.Contains(AppRoles.Manager, body.Data.Roles);
    }

    [Fact]
    public async Task Me_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
