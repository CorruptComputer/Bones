using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.GenericItem;

/// <summary>
///   DB Command for creating an Item Layout
/// </summary>
/// <param name="FriendlyIdPrefix"></param>
/// <param name="ProjectId"></param>
public sealed record CreateItemLayoutDbCommand(Guid ProjectId, string FriendlyIdPrefix) : IRequest<CommandResponse>;

internal class CreateItemLayoutDbCommandValidator : AbstractValidator<CreateItemLayoutDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateItemLayoutDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.FriendlyIdPrefix).NotNull().NotEmpty().MaximumLength(6).Matches(@"^[a-zA-Z]*$");

        return base.ValidateAsync(context, cancellation);
    }
}

internal class CreateItemLayoutDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateItemLayoutDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateItemLayoutDbCommand request, CancellationToken cancellationToken)
    {

        Project? project = await dbContext.Projects.FindAsync([request.ProjectId], cancellationToken);

        if (project == null)
        {
            return CommandResponse.Fail("Project does not exist");
        }

        GenericItemLayout newLayout = new()
        {
            ProjectId = project.Id,
            FriendlyIdPrefix = request.FriendlyIdPrefix
        };

        EntityEntry<GenericItemLayout> added = await dbContext.ItemLayouts.AddAsync(newLayout, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(added.Entity.Id);
    }
}