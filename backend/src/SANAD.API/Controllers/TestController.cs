using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SANAD.Application.Common;
using SANAD.Domain.Constants;

namespace SANAD.API.Controllers;

/// <summary>
/// Temporary endpoints used to verify authentication and role authorization.
/// </summary>
[ApiController]
[Route("api/test")]
public sealed class TestController : ControllerBase
{
    [HttpGet("authenticated")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public IActionResult Authenticated() =>
        Ok(ApiResponse<string>.Ok("Authenticated successfully."));

    [HttpGet("manager")]
    [Authorize(Roles = AppRoles.Manager)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public IActionResult Manager() =>
        Ok(ApiResponse<string>.Ok("Manager access granted."));
}
