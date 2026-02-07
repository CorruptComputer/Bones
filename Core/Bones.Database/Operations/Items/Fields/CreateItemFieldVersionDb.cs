using System.Data;
using Bones.Database.DbSets.Items.Fields;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Items.Fields;

/// <inheritdoc />
public sealed class CreateItemFieldVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateItemFieldVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating a new item field version
    /// </summary>
    /// <param name="ItemFieldId">Internal ID of the item field</param>
    /// <param name="Name"></param>
    /// <param name="IsRequired"></param>
    /// <param name="Type"></param>
    /// <param name="CanBeNegative"></param>
    /// <param name="PossibleValues"></param>
    public record Command(Guid ItemFieldId, string Name, bool IsRequired, FieldType Type,
        bool? CanBeNegative, Dictionary<string, StringValueMatchingType>? PossibleValues) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemFieldId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(512);
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.CanBeNegative).NotNull().When(x => x.Type is FieldType.Integer or FieldType.Decimal);
            RuleFor(x => x.PossibleValues).NotEmpty().When(x => x.Type == FieldType.ValueList);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        ItemField? field = await dbContext.ItemFields.FindAsync([request.ItemFieldId], cancellationToken);

        if (field is null || field.DeleteFlag)
        {
            return CommandResponse.Fail("Field not found");
        }

        EntityEntry<ItemFieldVersion> added = dbContext.ItemFieldVersions.Add(new()
        {
            ItemFieldId = field.Id,
            // Version numbers are increment only, going back to a previous version just creates a new version with the old values
            Version = (field.Current?.Version ?? 0) + 1,
            Name = request.Name,
            IsRequired = request.IsRequired,
            Type = request.Type,
            CanBeNegative = request.CanBeNegative
        });

        if (request.Type == FieldType.ValueList)
        {
            if (request.PossibleValues == null)
            {
                return CommandResponse.Fail("Possible values cannot be null for ValueList type");
            }

            IEnumerable<ItemFieldListEntry> possibleValues = request.PossibleValues.Select(x => new ItemFieldListEntry
            {
                ItemFieldVersionId = added.Entity.Id,
                Value = x.Key,
                MatchingType = x.Value
            });

            await dbContext.ItemFieldListEntries.AddRangeAsync(possibleValues, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ItemFieldVersion), added.Entity.Id);
    }
}
