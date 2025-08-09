using Bones.Shared.Enums;

namespace Bones.Api.Models.GenericItem;

/// <summary>
///   Model for the values of an item
/// </summary>
public sealed record ItemValueDisplayModel
{
    /// <summary>
    ///   The order that this field should be displayed in
    /// </summary>
    public required uint OrderNumber { get; init; }

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
}