using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SANAD.Application.Features.Auth;

namespace SANAD.Infrastructure.Identity;

public sealed class ExpiredRefreshTokenCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpiredRefreshTokenCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromDays(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(CleanupInterval);
        do
        {
            await CleanupExpiredTokensAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var tokenStore = scope.ServiceProvider.GetRequiredService<IRefreshTokenStore>();
            var deletedCount = await tokenStore.DeleteExpiredAsync(cancellationToken);
            if (deletedCount > 0)
            {
                logger.LogInformation("Removed {TokenCount} expired refresh tokens.", deletedCount);
            }
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Expired refresh-token cleanup failed.");
        }
        catch (DbException exception)
        {
            logger.LogError(exception, "Expired refresh-token cleanup failed.");
        }
    }
}
