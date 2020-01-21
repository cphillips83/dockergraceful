using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class GracePeriodManagerService : BackgroundService
{
    private readonly ILogger _logger;

    public GracePeriodManagerService(ILogger<GracePeriodManagerService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"GracePeriodManagerService is starting.");

        stoppingToken.Register(() =>
            _logger.LogInformation($" GracePeriod background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"GracePeriod task doing background work.");
            _logger.LogInformation("Ping");

            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation($"GracePeriod background task is stopping.");
    }
}