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
    ///   Is the field required?
    /// </summary>
    [JsonRequired]
    public required bool IsRequired { get; init; }

    /// <summary>
    ///   Type of the item field
    /// </summary>
    [JsonRequired]
    public required FieldType Type { get; init; }

    /// <summary>
    ///   If the field is numeric, can it be negative?
    /// </summary>
    public bool? CanBeNegative { get; init; }

    /// <summary>
    ///   If the field is a ValueList, the possible values
    /// </summary>
    public IEnumerable<KeyValuePair<string, StringValueMatchingType>>? PossibleValues { get; init; }

    /// <summary>
    ///   If the field is a GeoLocation, the type of geo location
    /// </summary>
    public GeoLocationType? GeoLocationType { get; init; }

    /// <summary>
    ///   If the field is a GeoLocation of type Address, the required address fields
    /// </summary>
    public AddressFields? RequiredAddressFields { get; init; }

    internal static GetLatestItemFieldVersionResponse FromInternal(GenericItemField field)
    {
        if (field.LatestVersion is null)
        {
            throw new InvalidOperationException("Field has no latest version");
        }

        return new()
        {
            FieldId = field.Id,
            FieldVersionId = field.LatestVersion.Id,
            Version = field.LatestVersion.Version,
            Name = field.LatestVersion.Name,
            IsRequired = field.LatestVersion.IsRequired,
            Type = field.LatestVersion.Type,
            CanBeNegative = field.LatestVersion.CanBeNegative,
            PossibleValues = field.LatestVersion.PossibleValues?.Select(x => new KeyValuePair<string, StringValueMatchingType>(x.Value, x.MatchingType)),
            GeoLocationType = field.LatestVersion.GeoLocationType,
            RequiredAddressFields = field.LatestVersion.RequiredAddressFields
        };
    }
}
