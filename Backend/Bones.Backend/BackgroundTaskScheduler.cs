namespace Bones.Backend;

public class BackgroundTaskScheduler : BackgroundService
{
    private readonly ILogger<BackgroundTaskScheduler> _logger;

    public BackgroundTaskScheduler(ILogger<BackgroundTaskScheduler> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("BackgroundTaskScheduler running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}