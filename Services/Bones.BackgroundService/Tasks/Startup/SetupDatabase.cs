using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Database.Operations.System;
using Bones.Database.Operations.System.TestingDataSetup;
using Bones.Logic.Features.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Backend.Models;

namespace Bones.BackgroundService.Tasks.Startup;

internal class SetupDatabase(ISender sender, BonesBackendConfiguration config) : StartupTaskBase(sender)
{
    protected override async Task RunTaskAsync(CancellationToken cancellationToken)
    {
        await Sender.Send(new SetupDb.Command(), cancellationToken);

        if (config.SetupForTesting)
        {
            CommandResponse testUserCreated = await Sender.Send(new SetupTestUserDb.Command(), cancellationToken);
            if (!testUserCreated.Success)
            {
                Log.Error("Failed to create test user: {Reasons}", testUserCreated.FailureReasons);
                return;
            }

            BonesUser? testUser = await Sender.Send(new GetUserByEmailDb.Query(DefaultValues.TEST_USER_EMAIL), cancellationToken);

            if (testUser is null)
            {
                return;
            }

            CommandResponse testProjectCreated = await Sender.Send(new CreateProjectWithPreset.Command("Test Project", ProjectPreset.Test, testUser, CreateTasks: true), cancellationToken);

            if (!testProjectCreated.Success)
            {
                Log.Error("Failed to create test project: {Reasons}", testProjectCreated.FailureReasons);
                return;
            }
        }
    }
}
