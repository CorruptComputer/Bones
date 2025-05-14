using Bones.Database.Operations.System.TestingDataSetup.Steps;

namespace Bones.Database.Operations.System.TestingDataSetup;

/// <inheritdoc />
public class SetupTestingDataDb(ISender sender)
    : IRequestHandler<SetupTestingDataDb.Command, CommandResponse>
{
    /// <summary>
    ///   Command for setting up default testing data
    /// </summary>
    public sealed record Command : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        CommandResponse testUser = await sender.Send(new SetupTestUserDb.Command(), cancellationToken);
        if (testUser.Id == null)
        {
            return CommandResponse.Fail("Failed to create test user.");
        }

        CommandResponse project = await sender.Send(new SetupTestProjectDb.Command(testUser.Id.Value), cancellationToken);
        if (project.Id == null)
        {
            return CommandResponse.Fail("Failed to create test project.");
        }

        return CommandResponse.Pass();
    }
}