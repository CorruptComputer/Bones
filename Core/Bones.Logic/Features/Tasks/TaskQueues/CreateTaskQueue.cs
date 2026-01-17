using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.OrganizationManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives;
using Bones.Database.Operations.TaskManagement.TaskQueues;
using Bones.Logic.Features.Initiatives;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.TaskQueues;

/// <inheritdoc />
public sealed class CreateTaskQueue(ISender sender) : IRequestHandler<CreateTaskQueue.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating a Queue.
    /// </summary>
    /// <param name="Name">Name of the queue</param>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(string Name, Guid InitiativeId, BonesUser RequestingUser) : IRequest<CommandResponse>;

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
        Initiative? initiative = await sender.Send(new GetInitiativesByIdDb.Query(request.InitiativeId), cancellationToken);

        if (initiative is null)
        {
            return CommandResponse.Fail("Initiative not found");
        }

        if (initiative.Project!.OwnerType == OwnershipType.User
            && initiative.Project.OwningUserId == request.RequestingUser.Id)
        {
            return await sender.Send(new CreateTaskQueueDb.Command(request.Name, request.InitiativeId), cancellationToken);
        }

        BonesOrganization? organization = await sender.Send(new GetOrganizationByIdDb.Query(initiative.Project.OwningOrganization!.Id), cancellationToken);
        // Don't want to give away that this org doesn't exist, instead just return forbidden.
        if (organization is null)
        {
            return CommandResponse.Forbid();
        }

        const string perm = BonesClaimTypes.Role.TaskQueue.CREATE_QUEUE;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasInitiativePermission.Query(initiative.Id, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return CommandResponse.Forbid();
        }

        return await sender.Send(new CreateTaskQueueDb.Command(request.Name, request.InitiativeId), cancellationToken);
    }
}
