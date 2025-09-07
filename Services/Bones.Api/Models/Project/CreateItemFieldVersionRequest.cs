using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.GenericItem;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   Request to create a new item field version
/// </summary>
[JsonSerializable(typeof(CreateItemFieldVersionRequest))]
public record CreateItemFieldVersionRequest
{
    /// <summary>
    ///   Name of the item field to create
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///  Is the field required?
    /// </summary>
    [JsonRequired]
    public required bool IsRequired { get; init; }

    /// <summary>
    ///  Type of the item field
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
    public List<KeyValuePair<string, StringValueMatchingType>>? PossibleValues { get; init; }

    /// <summary>
    ///   If the field is a GeoLocation, the type of geo location
    /// </summary>
    public GeoLocationType? GeoLocationType { get; init; }

    /// <summary>
    ///   If the field is a GeoLocation of type Address, the required address fields
    /// </summary>
    public AddressFields? RequiredAddressFields { get; init; }

    internal CreateItemFieldVersion.Command ToInternal(Guid itemFieldId, BonesUser user)
    {
        return new(itemFieldId, Name, IsRequired, Type, CanBeNegative, PossibleValues?.ToDictionary(), GeoLocationType, RequiredAddressFields, user);
    }
}