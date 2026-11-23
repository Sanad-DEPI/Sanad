using System.Net;
using System.Net.Http.Json;
using SANAD.Application.Common;
using SANAD.Domain.Constants;

namespace SANAD.IntegrationTests;

public sealed class ProtectedEndpointTests(SanadWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task AuthenticatedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/test/authenticated");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticatedEndpoint_WithToken_ReturnsOk()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Donor);
        var token = await LoginAsync(email, password);

        var response = await Client.SendAsync(GetWithToken("/api/test/authenticated", token));
        var body = await ReadApiAsync<string>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.Succeeded);
        Assert.Equal("Authenticated successfully.", body.Data);
    }

    [Fact]
    public async Task ManagerEndpoint_WithDonorToken_ReturnsForbidden()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Donor);
        var token = await LoginAsync(email, password);

        var response = await Client.SendAsync(GetWithToken("/api/test/manager", token));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ManagerEndpoint_WithManagerToken_ReturnsOk()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Manager);
        var token = await LoginAsync(email, password);

        var response = await Client.SendAsync(GetWithToken("/api/test/manager", token));
        var body = await ReadApiAsync<string>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.Succeeded);
        Assert.Equal("Manager access granted.", body.Data);
    }
}
