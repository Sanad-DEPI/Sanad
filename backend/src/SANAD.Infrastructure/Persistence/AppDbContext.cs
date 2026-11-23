using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SANAD.Infrastructure.Identity;

namespace SANAD.Infrastructure.Persistence;

/// <summary>
/// Identity-only database context for the base solution. Business tables are added later.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options);
