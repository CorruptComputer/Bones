namespace Bones.Shared.Extensions;

/// <summary>
///   Extensions to Task
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    ///   Await a task with a predefined timeout, if the timeout is reached an OperationCanceledException is thrown.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeout"></param>
    /// <param name="success"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <exception cref="OperationCanceledException"></exception>
    public static async Task AwaitWithTimeout<TSource>(this Task<TSource> task, TimeSpan timeout, Action<TSource> success)
    {
        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
        {
            success(task.Result);
        }
        else
        {
            throw new OperationCanceledException();
        }
    }
}