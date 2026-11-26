using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SANAD.API.Authorization;
using SANAD.Application.Common;
using SANAD.Application.Features.Auth;

namespace SANAD.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return Unauthorized(ApiResponse<LoginResponse>.Fail(result.Error ?? "Invalid email or password."));
        }

        return Ok(ApiResponse<LoginResponse>.Ok(result.Response!));
    }

    [HttpPost("register")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<RegistrationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<RegistrationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Register(
        [FromBody] RegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<RegistrationResponse>.Fail(result.Error!));
        }

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<RegistrationResponse>.Ok(result.Response!));
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid refresh token."));
        }

        return Ok(ApiResponse<LoginResponse>.Ok(result.Response!));
    }

    [HttpPost("logout")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        await authService.LogoutAsync(request, cancellationToken);
        return Ok(ApiResponse<string>.Ok("Logged out."));
    }
}
