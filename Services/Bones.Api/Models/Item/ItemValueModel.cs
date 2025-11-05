namespace Bones.Api.Models.Item;

/// <summary>
///   Model for the values of an item
/// </summary>
public sealed record ItemValueModel
{
    /// <summary>
    ///   ID of the item field
    /// </summary>
    public required Guid FieldVersionId { get; init; }

    /// <summary>
    ///   Value of the item field, depending on the type one of these will be filled
    /// </summary>
    public string? StrValue { get; init; }

    /// <summary>
    ///   Value of the item field, depending on the type one of these will be filled
    /// </summary>
    public long? IntValue { get; init; }

    /// <summary>
    ///   Value of the item field, depending on the type one of these will be filled
    /// </summary>
    public double? DecimalValue { get; init; }

    /// <summary>
    ///   Value of the item field, depending on the type one of these will be filled
    /// </summary>
    public DateTimeOffset? DateTimeValue { get; init; }

    /// <summary>
    ///   Value of the item field, depending on the type one of these will be filled
    /// </summary>
    public bool? BoolValue { get; init; }
}