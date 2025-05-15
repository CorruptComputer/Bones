using Bones.Database.DbSets.GenericItems;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.GenericItem;

/// <inheritdoc />
public class CreateItemLayoutVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateItemLayoutVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an item layout version
    /// </summary>
    /// <param name="ItemLayoutId"></param>
    /// <param name="Name"></param>
    /// <param name="EnabledFor"></param>
    /// <param name="FieldVersions"></param>
    public sealed record Command(Guid ItemLayoutId, string Name, ItemLayoutUses EnabledFor, Dictionary<uint, Guid> FieldVersions) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(512);
            RuleFor(x => x.EnabledFor).IsInEnum();
            RuleFor(x => x.FieldVersions).NotEmpty()
                .Must(x => x.All(f => f.Value != Guid.Empty)).WithMessage("Field version IDs cannot be empty")
                .Must(x => {
                    IEnumerable<IGrouping<Guid, KeyValuePair<uint, Guid>>> g = x.GroupBy(f => f.Value);
                    if (!g.All(f => f.Count() == 1))
                    {
                        return false;
                    }

                    return true;
                }).WithMessage("Field versions must be unique")
                .Must(x => x.All(f => f.Key < 0)).WithMessage("Field version order numbers cannot less than 0")
                .Must(x => x.Any(f => f.Key == 0)).WithMessage("Field version order numbers must begin with 0")
                .Must(x => x.All(f => f.Key < x.Count)).WithMessage("Field version order numbers must be within range")
                .Must(x => {
                    IEnumerable<IGrouping<uint, KeyValuePair<uint, Guid>>> g = x.GroupBy(f => f.Key);
                    if (!g.All(f => f.Count() == 1))
                    {
                        return false;
                    }

                    return true;
                }).WithMessage("Field version order numbers must be unique");
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        GenericItemLayout? layout = await dbContext.ItemLayouts.FindAsync([request.ItemLayoutId], cancellationToken);

        if (layout == null)
        {
            return CommandResponse.Fail("ItemLayout not found");
        }

        List<GenericItemFieldVersion> fieldVersions = await dbContext.ItemFieldVersions.Where(x => request.FieldVersions.Values.Contains(x.Id)).ToListAsync(cancellationToken);
        List<GenericItemField> fields = await dbContext.ItemFields.Where(f => fieldVersions.Select(v => v.Id).Contains(f.Id)).ToListAsync(cancellationToken);

        // Check that all fields found are in the same project as the layout
        if (fields.Any(f => f.Project.Id != layout.Project.Id))
        {
            return CommandResponse.Forbid();
        }

        if (fieldVersions.Count != request.FieldVersions.Count)
        {
            IEnumerable<Guid> missingFieldVersions = request.FieldVersions.Values.Except(fieldVersions.Select(x => x.Id));
            return CommandResponse.Fail($"Field versions not found: {string.Join(", ", missingFieldVersions)}");
        }

        GenericItemLayoutVersion lv = new()
        {
            ItemLayout = layout,
            Name = request.Name,
            EnabledFor = request.EnabledFor,
            Version = (layout.CurrentVersion?.Version ?? 0) + 1,
            CreateDateTime = DateTimeOffset.Now,
            FieldLinks = []
        };

        lv.FieldLinks.AddRange(request.FieldVersions.Select(fv => new GenericItemLayoutFieldVersionLink
        {
            OrderNumber = fv.Key,
            LayoutVersion = lv,
            FieldVersion = fieldVersions.Single(f => f.Id == fv.Value)
        }));

        EntityEntry<GenericItemLayoutVersion> added = dbContext.ItemLayoutVersions.Add(lv);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(added.Entity.Id);
    }
}