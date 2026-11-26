using SANAD.Application.Features.Auth;
using SANAD.Domain.Constants;
using SANAD.UnitTests.Fakes;

namespace SANAD.UnitTests.Features.Auth;

public sealed class AuthServiceTests
{
    private static readonly TokenResult Token = new("signed-token", DateTime.UtcNow.AddMinutes(15));
    private static readonly TokenResult RefreshToken = new("refresh-token", DateTime.UtcNow.AddDays(7));

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokenResponse()
    {
        var roles = new[] { AppRoles.Manager };
        var identity = new FakeIdentityService(
            AuthResult.Success("user-1", "manager@example.test", roles));
        var tokens = new FakeJwtTokenGenerator(Token);
        var refreshTokens = new FakeRefreshTokenStore(RefreshToken);
        var service = new AuthService(identity, tokens, refreshTokens);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "  manager@example.test  ",
            Password = "Test@12345"
        });

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);
        Assert.Equal(Token.Value, result.Response.Token);
        Assert.Equal(Token.ExpiresAtUtc, result.Response.ExpiresAtUtc);
        Assert.Equal(RefreshToken.Value, result.Response.RefreshToken);
        Assert.Equal(RefreshToken.ExpiresAtUtc, result.Response.RefreshTokenExpiresAtUtc);
        Assert.Equal("manager@example.test", result.Response.Email);
        Assert.Equal(roles, result.Response.Roles);
        Assert.Equal("manager@example.test", identity.CapturedEmail);
        Assert.Equal(1, tokens.CallCount);
        Assert.Equal("user-1", tokens.CapturedUserId);
        Assert.Equal(1, refreshTokens.CreateCallCount);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreRejected_DoesNotGenerateToken()
    {
        var identity = new FakeIdentityService(AuthResult.Failure("Invalid email or password."));
        var tokens = new FakeJwtTokenGenerator(Token);
        var refreshTokens = new FakeRefreshTokenStore(RefreshToken);
        var service = new AuthService(identity, tokens, refreshTokens);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@example.test",
            Password = "Test@12345"
        });

        Assert.False(result.Succeeded);
        Assert.Null(result.Response);
        Assert.Equal("Invalid email or password.", result.Error);
        Assert.Equal(0, tokens.CallCount);
        Assert.Equal(0, refreshTokens.CreateCallCount);
    }

    [Theory]
    [InlineData("", "Test@12345")]
    [InlineData("user@example.test", "")]
    public async Task LoginAsync_WithMissingCredentials_FailsBeforeCallingIdentity(
        string email,
        string password)
    {
        var identity = new FakeIdentityService(
            AuthResult.Success("user-1", email, [AppRoles.Donor]));
        var tokens = new FakeJwtTokenGenerator(Token);
        var service = new AuthService(identity, tokens, new FakeRefreshTokenStore(RefreshToken));

        var result = await service.LoginAsync(new LoginRequest { Email = email, Password = password });

        Assert.False(result.Succeeded);
        Assert.Equal("Email and password are required.", result.Error);
        Assert.Equal(0, identity.CallCount);
        Assert.Equal(0, tokens.CallCount);
    }

    [Fact]
    public async Task RegisterAsync_AssignsOnlyAnApprovedPublicRole()
    {
        var identity = new FakeIdentityService(AuthResult.Failure("Invalid email or password."));
        var service = new AuthService(
            identity,
            new FakeJwtTokenGenerator(Token),
            new FakeRefreshTokenStore(RefreshToken));

        var result = await service.RegisterAsync(new RegistrationRequest
        {
            Email = " donor@example.test ",
            Password = "Test@12345",
            AccountType = AppRoles.Donor
        });

        Assert.True(result.Succeeded);
        Assert.Equal(AppRoles.Donor, identity.CapturedPublicRole);
        Assert.Equal("donor@example.test", identity.CapturedEmail);
    }

    [Theory]
    [InlineData(AppRoles.Manager)]
    [InlineData(AppRoles.SuperAdmin)]
    [InlineData(AppRoles.DocumentReviewer)]
    public async Task RegisterAsync_RejectsStaffRoles(string role)
    {
        var identity = new FakeIdentityService(AuthResult.Failure("Invalid email or password."));
        var service = new AuthService(
            identity,
            new FakeJwtTokenGenerator(Token),
            new FakeRefreshTokenStore(RefreshToken));

        var result = await service.RegisterAsync(new RegistrationRequest
        {
            Email = "user@example.test",
            Password = "Test@12345",
            AccountType = role
        });

        Assert.False(result.Succeeded);
        Assert.Null(identity.CapturedPublicRole);
    }
}
