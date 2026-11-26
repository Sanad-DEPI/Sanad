using System.ComponentModel.DataAnnotations;

namespace SANAD.Application.Features.Auth;

public sealed class LogoutRequest
{
    [Required(ErrorMessage = "Refresh token is required.")]
    public string RefreshToken { get; set; } = string.Empty;
}
