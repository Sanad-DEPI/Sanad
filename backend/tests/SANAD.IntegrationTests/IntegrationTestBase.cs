using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SANAD.Application.Common;
using SANAD.Application.Features.Auth;
using SANAD.Infrastructure.Identity;

namespace SANAD.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<SanadWebApplicationFactory>
{
    protected const string TestPassword = "Test@12345";

    protected IntegrationTestBase(SanadWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected SanadWebApplicationFactory Factory { get; }

    protected HttpClient Client { get; }

    protected async Task<(string Email, string Password)> CreateUserAsync(params string[] roles)
    {
        using var scope = Factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in roles.Distinct())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleCreated = await roleManager.CreateAsync(new IdentityRole(role));
                Assert.True(
                    roleCreated.Succeeded,
                    string.Join("; ", roleCreated.Errors.Select(error => error.Code)));
            }
        }

        var email = $"test-{Guid.NewGuid():N}@example.test";
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var created = await users.CreateAsync(user, TestPassword);
        Assert.True(created.Succeeded, string.Join("; ", created.Errors.Select(error => error.Description)));

        foreach (var role in roles.Distinct())
        {
            var assigned = await users.AddToRoleAsync(user, role);
            Assert.True(assigned.Succeeded, string.Join("; ", assigned.Errors.Select(error => error.Code)));
        }

        return (email, TestPassword);
    }

    protected async Task<string> LoginAsync(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var body = await ReadApiAsync<LoginResponse>(response);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.Succeeded);
        Assert.NotNull(body.Data);
        Assert.False(string.IsNullOrWhiteSpace(body.Data.Token));

        return body.Data.Token;
    }

    protected static HttpRequestMessage GetWithToken(string url, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    protected static async Task<ApiResponse<T>> ReadApiAsync<T>(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        Assert.NotNull(body);
        return body;
    }
}
