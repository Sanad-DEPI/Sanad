using System.Net;
using System.Net.Http.Json;
using SANAD.Application.Common;
namespace SANAD.IntegrationTests;

public sealed class AuthRateLimitTests
{
    [Fact]
    public async Task Login_RejectsRequestsAbovePerIpLimit()
    {
        using var factory = new SanadWebApplicationFactory(loginPermitLimit: 10);
        using var client = factory.CreateClient();
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var allowed = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { email = "unknown@example.test", password = "Wrong@12345" });
            Assert.Equal(HttpStatusCode.Unauthorized, allowed.StatusCode);
        }

        var rejected = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "unknown@example.test", password = "Wrong@12345" });
        var body = await rejected.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.NotNull(body);
        Assert.False(body.Succeeded);
        Assert.Equal("Too many authentication attempts. Try again later.", body.Message);
    }
}
