using Serilog;
using Serilog.Events;

namespace Bones.Backend.PipelineBehaviors;

internal abstract class BackendPipelineBehaviorBase
{
    protected static readonly bool DebugLog = Log.IsEnabled(LogEventLevel.Debug);
}