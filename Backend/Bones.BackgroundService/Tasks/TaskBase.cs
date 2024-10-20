using Serilog.Events;

namespace Bones.BackgroundService.Tasks;

internal abstract class TaskBase(ISender sender) : Microsoft.Extensions.Hosting.BackgroundService
{
    protected readonly ISender Sender = sender;
    protected abstract TimeSpan Interval { get; }

    protected bool IsEnabled { get; set; } = true;

    protected abstract bool IsStartupOnlyTask { get; set; }

    protected abstract Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken);

    protected abstract Task RunTaskAsync(CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(Interval);
        while (!stoppingToken.IsCancellationRequested
               && await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (!await ShouldTaskRunAsync(stoppingToken))
            {
                continue;
            }

            try
            {
                if (Log.IsEnabled(LogEventLevel.Information))
                {
                    Log.Information("{Name} started at: {Time}", GetType().Name, DateTimeOffset.Now);
                }

                await RunTaskAsync(stoppingToken);

                if (Log.IsEnabled(LogEventLevel.Information))
                {
                    Log.Information("{Name} finished at: {Time}", GetType().Name, DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                // TODO: add to TaskErrors table
                IsEnabled = false;
                Log.Error(ex,
                    "{Time} | {Name} has unhandled exception: {ExceptionMessage}\n{ExceptionStackTrace}",
                    DateTimeOffset.Now, GetType().Name, ex.Message, ex.StackTrace);
            }
            finally
            {
                if (IsStartupOnlyTask)
                {
                    IsEnabled = false;

                    // Lets also cancel it
                    await CancellationTokenSource.CreateLinkedTokenSource(stoppingToken).CancelAsync();
                }
            }
        }
    }
}