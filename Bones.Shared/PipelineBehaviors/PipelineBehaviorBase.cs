using Serilog.Events;

namespace Bones.Shared.PipelineBehaviors;

/// <summary>
///     Base class for pipeline behaviors
/// </summary>
public abstract class PipelineBehaviorBase
{
    /// <summary>
    ///     Should debug logging be enabled?
    /// </summary>
    protected static readonly bool DebugLog = Log.IsEnabled(LogEventLevel.Debug);
}