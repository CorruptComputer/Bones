using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Projects.Initiatives;

/// <inheritdoc />
public sealed class QueueDeleteInitiativeById(ISender sender) : IRequestHandler<QueueDeleteInitiativeById.Command, CommandResponse>
{
    /// <summary>
    ///   Queues the deletion of the initiative
    /// </summary>
    /// <param name="InitiativeId"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid InitiativeId, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Initiative.DELETE_INITIATIVE;
        bool? hasPermission =
            await sender.Send(new UserHasInitiativePermission.Query(request.InitiativeId, request.RequestingUser, perm), cancellationToken);

        if (hasPermission != true)
        {
            return CommandResponse.Forbid();
        }

        return await sender.Send(new QueueDeleteInitiativeByIdDb.Command(request.InitiativeId), cancellationToken);
    }
}