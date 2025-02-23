using Bones.Database.DbSets.GenericItems;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.GenericItem;

/// <summary>
///   API response for the GetLatestItemFieldVersionAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetLatestItemFieldVersionResponse))]
public sealed record GetLatestItemFieldVersionResponse
{
    /// <summary>
    ///   ID of the item field
    /// </summary>
    [JsonRequired]
    public required Guid FieldId { get; init; }

    /// <summary>
    ///   ID of the item field version
    /// </summary>
    [JsonRequired]
    public required Guid FieldVersionId { get; init; }

    /// <summary>
    ///   Version number of the item field
    /// </summary>
    [JsonRequired]
    public required long Version { get; init; }

    /// <summary>
    ///   Name of the item field
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///   
    /// </summary>
    [JsonRequired]
    public required bool IsRequired { get; init; }

    /// <summary>
    ///   
    /// </summary>
    [JsonRequired]
    public required FieldType Type { get; init; }

    /// <summary>
    ///   
    /// </summary>
    public bool? CanBeNegative { get; init; }

    /// <summary>
    ///   
    /// </summary>
    public Dictionary<string, StringValueMatchingType>? PossibleValues { get; init; }

    /// <summary>
    ///   
    /// </summary>
    public GeoLocationType? GeoLocationType { get; init; }

    /// <summary>
    ///   
    /// </summary>
    public AddressFields? RequiredAddressFields { get; init; }

    internal static GetLatestItemFieldVersionResponse FromInternal(GenericItemField field)
    {
        if (field.CurrentVersion is null)
        {
            throw new InvalidOperationException("Field has no current version");
        }

        return new()
        {
            FieldId = field.Id,
            FieldVersionId = field.CurrentVersion.Id,
            Version = field.CurrentVersion.Version,
            Name = field.CurrentVersion.Name,
            IsRequired = field.CurrentVersion.IsRequired,
            Type = field.CurrentVersion.Type,
            CanBeNegative = field.CurrentVersion.CanBeNegative,
            PossibleValues = field.CurrentVersion.PossibleValues?.ToDictionary(x => x.Value, x => x.MatchingType),
            GeoLocationType = field.CurrentVersion.GeoLocationType,
            RequiredAddressFields = field.CurrentVersion.RequiredAddressFields
        };
    }
}
