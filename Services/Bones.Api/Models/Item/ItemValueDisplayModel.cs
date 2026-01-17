using Bones.Database.DbSets.Items;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Item;

/// <summary>
///   Model for the values of an item
/// </summary>
public sealed record ItemValueDisplayModel
{
    /// <summary>
    ///   The order that this field should be displayed in
    /// </summary>
    public required int OrderNumber { get; init; }

    /// <summary>
    ///   The ID of the field version this value is for
    /// </summary>
    public required Guid FieldVersionId { get; init; }

    /// <summary>
    ///   The name of the field
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///   The type of the field
    /// </summary>
    public required FieldType ValueType { get; init; }

    /// <summary>
    ///   Is this field required?
    /// </summary>
    public required bool IsRequired { get; init; }

    /// <summary>
    ///   If the field is a number, can it be negative?
    /// </summary>
    public bool? CanBeNegative { get; init; }

    /// <summary>
    ///   If this field is a value list, the possible values for the field
    /// </summary>
    public IEnumerable<string>? PossibleValues { get; init; }

    /// <summary>
    ///   The value of the field
    /// </summary>
    public string? Value { get; init; }

    internal static ItemValueDisplayModel FromInternal(int orderNumber, ItemFieldVersion itemFieldVersion, ItemValue value)
    {
        return new()
        {
            OrderNumber = orderNumber,
            FieldVersionId = itemFieldVersion.Id,
            Name = itemFieldVersion.Name,
            ValueType = itemFieldVersion.Type,
            IsRequired = itemFieldVersion.IsRequired,
            CanBeNegative = itemFieldVersion.CanBeNegative,
            PossibleValues = itemFieldVersion.PossibleValues?.Select(v => v.Value),
            Value = value.Value
        };
    }
}