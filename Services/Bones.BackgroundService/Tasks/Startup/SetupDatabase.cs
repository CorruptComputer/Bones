using Bones.Database.Operations.System;
using Bones.Logic.Features.System.TestingDataSetup;
using Bones.Shared.Backend.Models;

namespace Bones.BackgroundService.Tasks.Startup;

internal class SetupDatabase(ISender sender, BonesBackendConfiguration config) : StartupTaskBase(sender)
{
    protected override async Task RunTaskAsync(CancellationToken cancellationToken)
    {
        await Sender.Send(new SetupDb.Command(), cancellationToken);

        if (config.SetupForTesting && !config.ApiOnly)
        {
            await Sender.Send(new SetupTestData.Command(), cancellationToken);
        }
    }
}
