namespace SANAD.Application.Features.CurrentUser;

public sealed record CurrentUserDto(string Id, string Email, IReadOnlyList<string> Roles);
