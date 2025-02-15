using Bones.Database.DbSets.GenericItems;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.GenericItem;

/// <summary>
///   DB Command for creating an item layout version
/// </summary>
/// <param name="ItemLayoutId"></param>
/// <param name="Name"></param>
/// <param name="EnabledFor"></param>
/// <param name="FieldVersions"></param>
public sealed record CreateItemLayoutVersionDbCommand(Guid ItemLayoutId, string Name, ItemLayoutUses EnabledFor, List<Guid> FieldVersions) : IRequest<CommandResponse>;

internal class CreateItemLayoutVersionDbCommandValidator : AbstractValidator<CreateItemLayoutVersionDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateItemLayoutVersionDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}

internal class CreateItemLayoutVersionDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateItemLayoutVersionDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateItemLayoutVersionDbCommand request, CancellationToken cancellationToken)
    {
        GenericItemLayout? layout = await dbContext.ItemLayouts.FindAsync([request.ItemLayoutId], cancellationToken);

        if (layout == null)
        {
            return CommandResponse.Fail("ItemLayout not found");
        }

        List<GenericItemFieldVersion> fieldVersions = await dbContext.ItemFieldVersions.Where(x => request.FieldVersions.Contains(x.Id)).ToListAsync(cancellationToken);
        List<GenericItemField> fields = await dbContext.ItemFields.Where(f => fieldVersions.Select(v => v.Id).Contains(f.Id)).ToListAsync(cancellationToken);

        // Check that all fields found are in the same project as the layout
        if (fields.Any(f => f.ProjectId != layout.ProjectId)) 
        {
            return CommandResponse.Forbid();
        }

        if (fieldVersions.Count != request.FieldVersions.Count)
        {
            IEnumerable<Guid> missingFieldVersions = request.FieldVersions.Except(fieldVersions.Select(x => x.Id));
            return CommandResponse.Fail($"Field versions not found: {string.Join(", ", missingFieldVersions)}");
        }

        EntityEntry<GenericItemLayoutVersion> added = dbContext.ItemLayoutVersions.Add(new()
        {
            ItemLayoutId = layout.Id,
            Name = request.Name,
            EnabledFor = request.EnabledFor,
            Version = (layout.CurrentVersion?.Version ?? 0) + 1,
            CreateDateTime = DateTimeOffset.Now,
            Fields = fieldVersions
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(added.Entity.Id);
    }
}