using Serilog;
using Serilog.Events;

namespace Bones.Database.PipelineBehaviors;

internal abstract class DbPipelineBehaviorBase
{
    protected static readonly bool DebugLog = Log.IsEnabled(LogEventLevel.Debug);
}