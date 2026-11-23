namespace SANAD.Application.Features.Auth;

public interface IJwtTokenGenerator
{
    TokenResult Generate(string userId, string email, IReadOnlyList<string> roles);
}
