using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives.QueueDeleteInitiativeByIdDb;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Projects.Initiatives;

/// <summary>
///   Queues the deletion of the initiative
/// </summary>
/// <param name="InitiativeId"></param>
/// <param name="RequestingUser"></param>
public sealed record QueueDeleteInitiativeByIdCommand(Guid InitiativeId, BonesUser RequestingUser) : IRequest<CommandResponse>;

internal sealed class QueueDeleteInitiativeByIdCommandValidator : AbstractValidator<QueueDeleteInitiativeByIdCommand>
{

}

internal sealed class QueueDeleteInitiativeByIdHandler(ISender sender) : IRequestHandler<QueueDeleteInitiativeByIdCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueDeleteInitiativeByIdCommand request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Initiative.DELETE_INITIATIVE;
        bool? hasPermission =
            await sender.Send(new UserHasInitiativePermissionQuery(request.InitiativeId, request.RequestingUser, perm), cancellationToken);

        if (hasPermission != true)
        {
            return CommandResponse.Forbid();
        }
        
        return await sender.Send(new QueueDeleteInitiativeByIdDbCommand(request.InitiativeId), cancellationToken);
    }
}