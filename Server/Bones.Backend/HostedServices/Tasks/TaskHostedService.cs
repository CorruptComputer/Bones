using Bones.Backend.HostedServices.Tasks.Hourly;
using Bones.Database.Models;
using Bones.Database.Operations.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Bones.Backend.HostedServices.Tasks;

internal class TaskHostedService(ISender sender) : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly List<TaskBase> _registeredTasks = [
        // Variable
        // Hourly
        new CleanupExpiredEmailVerifications(sender),
        // Daily
        // Weekly
        // Monthly
    ];

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        Log.Information("Task Service running.");

        await CheckForDnfTasks();

        _timer = new(CheckForTasksReadyToRun, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
    }

    private Task CheckForDnfTasks()
    {
        return Task.CompletedTask;
        //List<long>? unclosedTaskHistories = await sender.Send(new GetUnclosedTaskHistoriesDbQuery());

        //if (unclosedTaskHistories != null)
        //{
        //    foreach (long taskHistoryId in unclosedTaskHistories)
        //    {
        //        await sender.Send(new SetTaskDnfDbCommand(taskHistoryId));
        //    }
        //}

        //List<string>? runningTasks = await sender.Send(new GetRunningTasksDbQuery());
        //if (runningTasks != null)
        //{
        //    foreach (string taskName in runningTasks)
        //    {
        //        await sender.Send(new SetTaskNotRunningDbCommand(taskName));
        //    }
        //}
    }

    private void CheckForTasksReadyToRun(object? state)
    {
        //await Parallel.ForEachAsync(_registeredTasks, async (task, cancellationToken) =>
        //{
        //    if (await task.ShouldRunAsync(cancellationToken))
        //    {
        //        long? taskHistoryId = null;
        //        try
        //        {
        //            DbCommandResponse startTask =
        //                await sender.Send(new SetTaskStartedDbCommand(task.TaskName), cancellationToken);

        //            if (!startTask.Success || startTask.Id == null)
        //            {
        //                // Nope out of there
        //                return;
        //            }

        //            taskHistoryId = startTask.Id;
        //            
        //            await task.RunAsync(cancellationToken);
        //            
        //            await sender.Send(new SetTaskFinishedDbCommand(taskHistoryId.Value, task.GetNextRunTime()), cancellationToken);
        //        }
        //        catch (Exception e)
        //        {
        //            Log.Fatal(e, $"[{task.TaskName}] Task failed, see exception for more info. TASK WILL BE DISABLED AND MUST BE MANUALLY RE-ENABLED");
        //            if (taskHistoryId != null)
        //            {
        //                await sender.Send(new SetTaskFinishedDbCommand(taskHistoryId.Value, null, e.Message), cancellationToken);
        //            }
        //        }
        //    }
        //});
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        Log.Information("Task Service is stopping.");
        _timer?.Dispose();
        _timer = null;

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}