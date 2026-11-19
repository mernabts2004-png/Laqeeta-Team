using Laqeeta.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Laqeeta.API.Services
{
    public class CentralStoreroomService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CentralStoreroomService> _logger;

        public CentralStoreroomService(
            IServiceScopeFactory scopeFactory,
            ILogger<CentralStoreroomService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    var cutoffDate = DateTime.UtcNow.AddDays(-30);

                    var items = await context.Items
                        .Where(i =>
                            i.StorageLocation == "Station" &&
                            i.Status == "Found" &&
                            i.FoundAt <= cutoffDate)
                        .ToListAsync(stoppingToken);

                    foreach (var item in items)
                    {
                        item.StorageLocation = "CentralStoreroom";
                        item.MovedToCentralAt = DateTime.UtcNow;
                    }

                    if (items.Count > 0)
                    {
                        await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation(
                            "{Count} items moved to the central storeroom.",
                            items.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An error occurred while moving items to the central storeroom.");
                }

                await Task.Delay(
                    TimeSpan.FromHours(24),
                    stoppingToken);
            }
        }
    }
}