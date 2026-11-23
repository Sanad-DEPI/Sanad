using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SANAD.Application.Common;
using SANAD.Application.Features.CurrentUser;

namespace SANAD.API.Controllers;

/// <summary>
/// Reference feature: Domain -> Application -> Infrastructure -> API.
/// </summary>
[ApiController]
[Route("api")]
public sealed class CurrentUserController(ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CurrentUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value
                     ?? User.FindFirst("nameid")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse<CurrentUserDto>.Fail("Token has no user identifier."));
        }

        var user = await currentUserService.GetAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound(ApiResponse<CurrentUserDto>.Fail("User not found."));
        }

        return Ok(ApiResponse<CurrentUserDto>.Ok(user));
    }
}
