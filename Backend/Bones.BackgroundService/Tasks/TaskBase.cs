namespace Bones.BackgroundService.Tasks;

internal abstract class TaskBase<T>(ILogger<T> logger, ISender sender) : Microsoft.Extensions.Hosting.BackgroundService
{
    protected readonly ISender Sender = sender;
    protected abstract TimeSpan? Interval { get; }

    protected bool IsEnabled { get; set; } = true;

    protected abstract Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken);

    protected abstract Task RunTaskAsync(CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using PeriodicTimer timer = new(Interval ?? TimeSpan.MaxValue);
        while (!cancellationToken.IsCancellationRequested
               && await timer.WaitForNextTickAsync(cancellationToken))
        {
            if (!await ShouldTaskRunAsync(cancellationToken))
            {
                continue;
            }

            try
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("{Name} started at: {Time}", typeof(T).Name, DateTimeOffset.Now);
                }

                await RunTaskAsync(cancellationToken);

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("{Name} finished at: {Time}", typeof(T).Name, DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                // TODO: add to TaskErrors table
                IsEnabled = false;
                logger.LogError(ex,
                    "{Time} | {Name} has unhandled exception: {ExceptionMessage}\n{ExceptionStackTrace}",
                    DateTimeOffset.Now, typeof(T).Name, ex.Message, ex.StackTrace);
            }
            finally
            {
                // Startup only task
                if (Interval is null)
                {
                    IsEnabled = false;

                    // Lets also cancel it to clear this from memory
                    await CancellationTokenSource.CreateLinkedTokenSource(cancellationToken).CancelAsync();
                }
            }
        }
    }
}