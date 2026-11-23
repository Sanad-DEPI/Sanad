using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SANAD.Application.Features.Auth;
using SANAD.Infrastructure.Configuration;

namespace SANAD.Infrastructure.Identity;

public sealed class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
{
    public TokenResult Generate(string userId, string email, IReadOnlyList<string> roles)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.Secret))
        {
            throw new InvalidOperationException("Jwt:Secret must be configured.");
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
