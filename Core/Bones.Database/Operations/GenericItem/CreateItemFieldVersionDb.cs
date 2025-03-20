using System.Data;
using Bones.Database.DbSets.GenericItems;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.GenericItem;

/// <inheritdoc />
public sealed class CreateItemFieldVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateItemFieldVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a new item field version
    /// </summary>
    /// <param name="ItemFieldId">Internal ID of the item field</param>
    /// <param name="Name"></param>
    /// <param name="IsRequired"></param>
    /// <param name="Type"></param>
    /// <param name="CanBeNegative"></param>
    /// <param name="PossibleValues"></param>
    /// <param name="GeoLocationType"></param>
    /// <param name="RequiredAddressFields"></param>
    public record Command(Guid ItemFieldId, string Name, bool IsRequired, FieldType Type,
        bool? CanBeNegative, Dictionary<string, StringValueMatchingType>? PossibleValues,
        GeoLocationType? GeoLocationType, AddressFields? RequiredAddressFields) : IRequest<CommandResponse>;

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
            RuleFor(x => x.GeoLocationType).NotNull().When(x => x.Type == FieldType.GeoLocation);
            RuleFor(x => x.RequiredAddressFields).NotNull().When(x => x.GeoLocationType == GeoLocationType.Address);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        GenericItemField? field = await dbContext.ItemFields.FindAsync([request.ItemFieldId], cancellationToken);

        if (field is null || field.DeleteFlag)
        {
            return CommandResponse.Fail("Field not found");
        }

        EntityEntry<GenericItemFieldVersion> added = dbContext.ItemFieldVersions.Add(new()
        {
            GenericItemField = field,
            // Version numbers are increment only, going back to a previous version just creates a new version with the old values
            Version = (field.CurrentVersion?.Version ?? 0) + 1,
            Name = request.Name,
            IsRequired = request.IsRequired,
            Type = request.Type,
            CanBeNegative = request.CanBeNegative,
            GeoLocationType = request.GeoLocationType,
            RequiredAddressFields = request.RequiredAddressFields
        });

        if (request.Type == FieldType.ValueList)
        {
            added.Entity.PossibleValues = request.PossibleValues?.Select(x => new GenericItemFieldListEntry
            {
                GenericItemFieldVersionId = added.Entity.Id,
                Value = x.Key,
                MatchingType = x.Value
            }).ToList();

            dbContext.ItemFieldVersions.Update(added.Entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(added.Entity.Id);
    }
}
