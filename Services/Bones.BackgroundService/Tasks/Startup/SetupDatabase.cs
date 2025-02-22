using Bones.Database.Operations.System;

namespace Bones.BackgroundService.Tasks.Startup;

internal class SetupDatabase(ISender sender) : StartupTaskBase(sender)
{
    protected override async Task RunTaskAsync(CancellationToken cancellationToken)
    {
        await Sender.Send(new SetupDb.Command(), cancellationToken);
    }
}
