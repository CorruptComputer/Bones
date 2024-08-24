namespace Bones.Backend.Tasks;

public abstract class TaskBase<T>(ILogger<T> logger) : BackgroundService
{
    protected abstract TimeSpan? Interval { get; init; }

    protected abstract Task RunTaskAsync(CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("{Name} started at: {Time}", typeof(T).Name, DateTimeOffset.Now);
            }

            try
            {
                await RunTaskAsync(stoppingToken);

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("{Name} finished at: {Time}", typeof(T).Name, DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Time} | {Name} has unhandled exception: {ExceptionMessage}\n{ExceptionStackTrace}", DateTimeOffset.Now, typeof(T).Name, ex.Message, ex.StackTrace);
            }

            if (!Interval.HasValue)
            {
                await CancellationTokenSource.CreateLinkedTokenSource(stoppingToken).CancelAsync();
            }
            else
            {
                await Task.Delay(Interval.Value.Milliseconds, stoppingToken);
            }
        }
    }
}