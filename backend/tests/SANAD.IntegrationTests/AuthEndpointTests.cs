using System.Net;
using System.Net.Http.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SANAD.Application.Common;
using SANAD.Application.Features.Auth;
using SANAD.Domain.Constants;
using SANAD.Infrastructure.Identity;
using SANAD.Infrastructure.Persistence;

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

        var unknownAccount = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = $"missing-{Guid.NewGuid():N}@example.test", password = "Wrong@12345" });
        var unknownBody = await ReadApiAsync<LoginResponse>(unknownAccount);
        Assert.Equal(HttpStatusCode.Unauthorized, unknownAccount.StatusCode);
        Assert.Equal(body.Message, unknownBody.Message);
    }

    [Fact]
    public async Task Login_UsesFifteenMinuteAccessTokenAndReturnsRefreshToken()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Donor);

        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var body = await ReadApiAsync<LoginResponse>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body.Data);
        Assert.InRange(
            body.Data.ExpiresAtUtc - DateTime.UtcNow,
            TimeSpan.FromMinutes(14.8),
            TimeSpan.FromMinutes(15));
        Assert.False(string.IsNullOrWhiteSpace(body.Data.RefreshToken));
        Assert.True(body.Data.RefreshTokenExpiresAtUtc > body.Data.ExpiresAtUtc);

        using var scope = Factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await users.FindByEmailAsync(email);
        Assert.NotNull(user);
        var storedToken = await scope.ServiceProvider
            .GetRequiredService<AppDbContext>()
            .UserTokens
            .SingleAsync(token => token.UserId == user.Id && token.LoginProvider == "SANAD.RefreshToken");
        Assert.Equal(
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(body.Data.RefreshToken))),
            storedToken.Name);
        Assert.NotEqual(body.Data.RefreshToken, storedToken.Value);
    }

    [Fact]
    public async Task Login_RejectsAccountWithMultiplePublicRoles()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Donor, AppRoles.Beneficiary);

        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_FiveFailedAttemptsLockAccount_AndValidLoginResetsCounter()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Beneficiary);

        await Client.PostAsJsonAsync("/api/auth/login", new { email, password = "Wrong@12345" });
        var correctLogin = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        Assert.Equal(HttpStatusCode.OK, correctLogin.StatusCode);

        using (var scope = Factory.Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await users.FindByEmailAsync(email);
            Assert.NotNull(user);
            Assert.Equal(0, await users.GetAccessFailedCountAsync(user));
        }

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var failed = await Client.PostAsJsonAsync("/api/auth/login", new { email, password = "Wrong@12345" });
            Assert.Equal(HttpStatusCode.Unauthorized, failed.StatusCode);
        }

        using (var scope = Factory.Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await users.FindByEmailAsync(email);
            Assert.NotNull(user);
            Assert.True(await users.IsLockedOutAsync(user));
        }

        var lockedLogin = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var lockedBody = await ReadApiAsync<LoginResponse>(lockedLogin);
        Assert.Equal(HttpStatusCode.Unauthorized, lockedLogin.StatusCode);
        Assert.Equal("Invalid email or password.", lockedBody.Message);
    }

    [Theory]
    [InlineData(AppRoles.Donor)]
    [InlineData(AppRoles.Beneficiary)]
    public async Task Register_AssignsOnlySelectedPublicRole(string role)
    {
        var email = $"registration-{Guid.NewGuid():N}@example.test";
        var response = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = TestPassword,
            accountType = role
        });
        var body = await ReadApiAsync<RegistrationResponse>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(role, body.Data?.AccountType);
        Assert.Equal(role == AppRoles.Donor, body.Data?.RequiresEmailVerification);

        using var scope = Factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await users.FindByEmailAsync(email);
        Assert.NotNull(user);
        Assert.Equal(new[] { role }, await users.GetRolesAsync(user));
        Assert.False(user.EmailConfirmed);
    }

    [Theory]
    [InlineData(AppRoles.Manager)]
    [InlineData(AppRoles.SuperAdmin)]
    [InlineData(AppRoles.DocumentReviewer)]
    [InlineData("GuestDonor")]
    public async Task Register_RejectsStaffAndUnrecognizedRoles(string role)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email = $"invalid-registration-{Guid.NewGuid():N}@example.test",
            password = TestPassword,
            accountType = role
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_CannotChangeExistingPublicAccountType()
    {
        var email = $"existing-{Guid.NewGuid():N}@example.test";
        var created = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = TestPassword,
            accountType = AppRoles.Donor
        });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var attemptedChange = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = TestPassword,
            accountType = AppRoles.Beneficiary
        });
        Assert.Equal(HttpStatusCode.BadRequest, attemptedChange.StatusCode);

        using var scope = Factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await users.FindByEmailAsync(email);
        Assert.NotNull(user);
        Assert.Equal(new[] { AppRoles.Donor }, await users.GetRolesAsync(user));
    }

    [Fact]
    public async Task Refresh_RotatesTokenAndRejectsReuse()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Beneficiary);
        var login = await LoginResponseAsync(email, password);

        var firstRotation = await Client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = login.RefreshToken });
        var rotated = await ReadApiAsync<LoginResponse>(firstRotation);

        Assert.Equal(HttpStatusCode.OK, firstRotation.StatusCode);
        Assert.NotNull(rotated.Data);
        Assert.NotEqual(login.RefreshToken, rotated.Data.RefreshToken);

        var reused = await Client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = login.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, reused.StatusCode);

        var nextRotation = await Client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = rotated.Data.RefreshToken });
        Assert.Equal(HttpStatusCode.OK, nextRotation.StatusCode);
    }

    [Fact]
    public async Task Refresh_RejectsExpiredTokenAndRemovesIt()
    {
        var (email, _) = await CreateUserAsync(AppRoles.Beneficiary);
        const string expiredToken = "expired-test-refresh-token";
        var expiredHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(expiredToken)));
        using (var scope = Factory.Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await users.FindByEmailAsync(email);
            Assert.NotNull(user);

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.UserTokens.Add(new IdentityUserToken<string>
            {
                UserId = user.Id,
                LoginProvider = "SANAD.RefreshToken",
                Name = expiredHash,
                Value = DateTime.UtcNow.AddMinutes(-1).ToString("O", CultureInfo.InvariantCulture)
            });
            await dbContext.SaveChangesAsync();
        }

        var response = await Client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken = expiredToken });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        using var verificationScope = Factory.Services.CreateScope();
        var remainingToken = await verificationScope.ServiceProvider
            .GetRequiredService<AppDbContext>()
            .UserTokens
            .SingleOrDefaultAsync(token =>
                token.LoginProvider == "SANAD.RefreshToken" && token.Name == expiredHash);
        Assert.Null(remainingToken);
    }

    [Fact]
    public async Task Logout_RevokesRefreshToken()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Beneficiary);
        var login = await LoginResponseAsync(email, password);

        var logout = await Client.PostAsJsonAsync(
            "/api/auth/logout",
            new { refreshToken = login.RefreshToken });
        var refresh = await Client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = login.RefreshToken });

        Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    [Fact]
    public async Task SuperAdminProvisioning_IsIdempotentAndDoesNotResetPassword()
    {
        using var scope = Factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
            .CreateLogger("SANAD.Infrastructure.Identity.IdentitySeeder");
        var user = await users.FindByEmailAsync("initial-superadmin@example.test");
        Assert.NotNull(user);
        var originalHash = user.PasswordHash;

        await IdentitySeeder.SeedAsync(
            roles,
            users,
            new SuperAdminSeedOptions("initial-superadmin@example.test", "Different@12345"),
            manager: null,
            logger: logger);

        var reloaded = await users.FindByEmailAsync("initial-superadmin@example.test");
        Assert.NotNull(reloaded);
        Assert.Equal(originalHash, reloaded.PasswordHash);
        Assert.True(await users.CheckPasswordAsync(reloaded, "Test@12345"));
        Assert.False(await users.CheckPasswordAsync(reloaded, "Different@12345"));
        Assert.Equal(new[] { AppRoles.SuperAdmin }, await users.GetRolesAsync(reloaded));
        foreach (var role in AppRoles.All)
        {
            Assert.True(await roles.RoleExistsAsync(role));
        }
    }

    [Fact]
    public async Task SuperAdminProvisioning_DoesNotElevateAnExistingPublicAccount()
    {
        var (email, _) = await CreateUserAsync(AppRoles.Donor);
        using var scope = Factory.Services.CreateScope();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            IdentitySeeder.SeedAsync(
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
                new SuperAdminSeedOptions(email, "Different@12345"),
                manager: null,
                logger: scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                    .CreateLogger("SANAD.Infrastructure.Identity.IdentitySeeder")));

        Assert.Contains("incompatible roles", exception.Message.ToLowerInvariant());
    }

    [Fact]
    public async Task SuperAdminProvisioning_FailsWhenCredentialsAreMissing()
    {
        using var scope = Factory.Services.CreateScope();
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            IdentitySeeder.SeedAsync(
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
                new SuperAdminSeedOptions(string.Empty, string.Empty),
                manager: null,
                logger: scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                    .CreateLogger("SANAD.Infrastructure.Identity.IdentitySeeder")));

        Assert.Contains("credentials are required", exception.Message.ToLowerInvariant());
    }

    [Fact]
    public async Task Refresh_ConcurrentRequestsCannotBothRotateOneToken()
    {
        var (email, password) = await CreateUserAsync(AppRoles.Beneficiary);
        var login = await LoginResponseAsync(email, password);

        var firstRequest = Client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = login.RefreshToken });
        var secondRequest = Client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = login.RefreshToken });
        var responses = await Task.WhenAll(firstRequest, secondRequest);

        Assert.Single(responses, response => response.StatusCode == HttpStatusCode.OK);
        Assert.Single(responses, response => response.StatusCode == HttpStatusCode.Unauthorized);
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

    private async Task<LoginResponse> LoginResponseAsync(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var body = await ReadApiAsync<LoginResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body.Data);
        return body.Data;
    }
}
