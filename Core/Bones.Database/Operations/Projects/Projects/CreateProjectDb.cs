using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Organizations;
using Bones.Database.DbSets.Projects;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Projects.Projects;

/// <inheritdoc />
public sealed class CreateProjectDb(BonesDbContext dbContext) : IRequestHandler<CreateProjectDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating a Project.
    /// </summary>
    /// <param name="Name">Name of the project</param>
    /// <param name="RequestingUser">The user requesting this projects creation</param>
    /// <param name="Organization">Optionally, the organization this project should belong to</param>
    public sealed record Command(string Name, BonesUser RequestingUser, BonesOrganization? Organization = null) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Project name is required.");
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Project project = new()
        {
            Name = request.Name
        };

        if (request.Organization == null)
        {
            project.OwnerType = OwnershipType.User;
            project.OwningUserId = request.RequestingUser.Id;
        }
        else
        {
            project.OwnerType = OwnershipType.Organization;
            project.OwningOrganizationId = request.Organization.Id;
        }


        EntityEntry<Project> created = await dbContext.Projects.AddAsync(project, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(Project), created.Entity.Id);
    }
}