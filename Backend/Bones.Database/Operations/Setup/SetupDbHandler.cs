using Bones.Shared.Backend.Models;

namespace Bones.Database.Operations.Setup;

public sealed class SetupDbHandler(ISender sender) : IRequestHandler<SetupDbHandler.Command, CommandResponse>
{
    public sealed record Command : IRequest<CommandResponse>;

    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        await sender.Send(new SetupSystemAdminUserAndRoleHandler.Command(), cancellationToken);

        return new()
        {
            Success = true
        };
    }
}