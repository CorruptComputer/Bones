namespace Bones.Api.Models.GenericItem;

/// <summary>
///   Model for the values of an item
/// </summary>
public sealed record ItemValueModel
{
    /// <summary>
    ///
    /// </summary>
    public required Guid FieldVersionId { get; init; }

    /// <summary>
    ///
    /// </summary>
    public string? StrValue { get; init; }

    /// <summary>
    ///
    /// </summary>
    public long? IntValue { get; init; }

    /// <summary>
    ///
    /// </summary>
    public double? DecimalValue { get; init; }

    /// <summary>
    ///
    /// </summary>
    public DateTimeOffset? DateTimeValue { get; init; }

    /// <summary>
    ///
    /// </summary>
    public bool? BoolValue { get; init; }
}