using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.Accounts;
using Bones.Database.Operations.System.TestingDataSetup;
using Bones.Logic.Features.Projects;
using Bones.Shared.Backend.Enums;
using Serilog;

namespace Bones.Logic.Features.System.TestingDataSetup;

/// <inheritdoc />
public class SetupTestData(ISender sender) : IRequestHandler<SetupTestData.Command, CommandResponse>
{
    /// <summary>
    ///   Command for setting up the database with testing data.
    /// </summary>
    public sealed record Command : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        // Setup the default test user
        CommandResponse testUserCreated = await sender.Send(new SetupDefaultTestUserDb.Command(), cancellationToken);
        if (!testUserCreated.Success)
        {
            Log.Error("Failed to create test user: {Reasons}", testUserCreated.FailureReasons);
            return CommandResponse.Fail();
        }

        BonesUser? testUser = await sender.Send(new GetUserByEmailDb.Query(DefaultValues.TestUsers.TEST_USER_EMAIL), cancellationToken);
        if (testUser is null)
        {
            return CommandResponse.Fail();
        }

        // Create a project for them
        CommandResponse testProjectCreated = await sender.Send(new CreateProjectWithPreset.Command("Test Project", ProjectPreset.Test, testUser, CreateTasks: true), cancellationToken);
        if (!testProjectCreated.Success)
        {
            Log.Error("Failed to create test project: {Reasons}", testProjectCreated.FailureReasons);
            return CommandResponse.Fail();
        }

        // Create the other test users
        CommandResponse changePasswordUserCreated = await sender.Send(new SetupChangePasswordTestUserDb.Command(), cancellationToken);
        if (!changePasswordUserCreated.Success)
        {
            Log.Error("Failed to create change password test user: {Reasons}", changePasswordUserCreated.FailureReasons);
            return CommandResponse.Fail();
        }

        CommandResponse changeEmailUserCreated = await sender.Send(new SetupChangeEmailTestUserDb.Command(), cancellationToken);
        if (!changeEmailUserCreated.Success)
        {
            Log.Error("Failed to create change email test user: {Reasons}", changeEmailUserCreated.FailureReasons);
            return CommandResponse.Fail();
        }

        return CommandResponse.Pass();
    }
}
