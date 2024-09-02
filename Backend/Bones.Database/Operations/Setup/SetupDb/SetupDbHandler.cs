using Bones.Database.Operations.Setup.SetupSystemAdminUserAndRole;

namespace Bones.Database.Operations.Setup.SetupDb;

internal sealed class SetupDbHandler(ISender sender) : IRequestHandler<SetupDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(SetupDbCommand request, CancellationToken cancellationToken)
    {
        await sender.Send(new SetupSystemAdminUserAndRoleCommand(), cancellationToken);

        return CommandResponse.Pass();
    }
}