using Microsoft.AspNetCore.Identity;

namespace SANAD.Infrastructure.Identity;

/// <summary>
/// Identity-backed user. Identity tables are the only database tables in the base solution.
/// </summary>
public sealed class ApplicationUser : IdentityUser;
