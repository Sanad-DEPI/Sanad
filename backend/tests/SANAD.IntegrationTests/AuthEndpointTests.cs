using System.Net;
using System.Net.Http.Json;
using SANAD.Application.Common;
using SANAD.Application.Features.Auth;
using SANAD.Domain.Constants;

namespace SANAD.IntegrationTests;

public sealed class AuthEndpointTests(SanadWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenAndRoles()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Manager);

        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var body = await ReadApiAsync<LoginResponse>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.Succeeded);
        Assert.NotNull(body.Data);
        Assert.False(string.IsNullOrWhiteSpace(body.Data.Token));
        Assert.Equal(email, body.Data.Email);
        Assert.Contains(AppRoles.Manager, body.Data.Roles);
        Assert.True(body.Data.ExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var (email, _) = await CreateUserAsync(AppRoles.Donor);

        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password = "Wrong@12345" });
        var body = await ReadApiAsync<LoginResponse>(response);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(body.Succeeded);
        Assert.Equal("Invalid email or password.", body.Message);
        Assert.Null(body.Data);
    }

    [Fact]
    public async Task Login_WithMissingEmail_ReturnsBadRequestWithEnvelope()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new { password = "Test@12345" });
        var body = await ReadApiAsync<object>(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(body.Succeeded);
        Assert.Equal("Validation failed.", body.Message);
        Assert.Contains(body.Errors, error => error.Contains("Email", StringComparison.OrdinalIgnoreCase));
    }
}
