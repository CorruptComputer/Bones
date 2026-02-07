using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Organizations;
using Bones.Database.Operations.Organizations;
using Bones.Database.Operations.Projects.Projects;
using Bones.Logic.Features.Organizations;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Projects;

/// <inheritdoc />
public sealed class CreateProject(ISender sender) : IRequestHandler<CreateProject.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating a Project.
    /// </summary>
    /// <param name="Name">Name of the project</param>
    /// <param name="RequestingUser">The user requesting this project be created</param>
    /// <param name="OrganizationId">Optionally, the organization this project should belong to.</param>
    public record Command(string Name, BonesUser RequestingUser, Guid? OrganizationId = null) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        if (!request.OrganizationId.HasValue)
        {
            return await sender.Send(new CreateProjectDb.Command(request.Name, request.RequestingUser), cancellationToken);
        }

        BonesOrganization? organization = await sender.Send(new GetOrganizationByIdDb.Query(request.OrganizationId.Value), cancellationToken);
        // Don't want to give away that this org doesn't exist, instead just return forbidden.
        if (organization is null)
        {
            return CommandResponse.Forbid();
        }

        const string perm = BonesClaimTypes.Role.Project.CREATE_PROJECT;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasOrganizationPermission.Query(organization.Id, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return CommandResponse.Forbid();
        }

        return await sender.Send(new CreateProjectDb.Command(request.Name, request.RequestingUser, organization), cancellationToken);
    }
}