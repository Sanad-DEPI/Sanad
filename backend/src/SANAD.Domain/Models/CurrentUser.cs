namespace SANAD.Domain.Models;

/// <summary>
/// Domain representation of the currently authenticated user. Not persisted.
/// </summary>
public sealed record CurrentUser(string Id, string Email, IReadOnlyList<string> Roles);
