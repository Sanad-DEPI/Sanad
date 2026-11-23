using System.Net;

namespace SANAD.IntegrationTests;

public sealed class HealthEndpointTests(SanadWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await Client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
