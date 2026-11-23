using Microsoft.AspNetCore.Mvc;
using SANAD.Application.Common;
using SANAD.Application.Features.Auth;

namespace SANAD.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return Unauthorized(ApiResponse<LoginResponse>.Fail(result.Error ?? "Invalid email or password."));
        }

        return Ok(ApiResponse<LoginResponse>.Ok(result.Response!));
    }
}
