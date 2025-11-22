using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Item;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   Request to create a new item field
/// </summary>
[JsonSerializable(typeof(CreateItemFieldRequest))]
public record CreateItemFieldRequest
{
    /// <summary>
    ///   The ID of the project to create this field in
    /// </summary>
    [JsonRequired]
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   Name of the item field to create
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///   Is this field required to have a value?
    /// </summary>
    [JsonRequired]
    public required bool IsRequired { get; init; }

    /// <summary>
    ///   The type of data this field will hold
    /// </summary>
    [JsonRequired]
    public required FieldType Type { get; init; }

    /// <summary>
    ///   If the Type of this field is either an Integer or Decimal, can it be negative?
    /// </summary>
    public bool? CanBeNegative { get; init; }

    /// <summary>
    ///   If the Type of this field is a ValueList, the possible values this can have
    /// </summary>
    public List<KeyValuePair<string, StringValueMatchingType>>? PossibleValues { get; init; }

    /// <summary>
    ///   If the Type of this field is a GeoLocation, the type of GeoLocation this is
    /// </summary>
    public GeoLocationType? GeoLocationType { get; init; }

    /// <summary>
    ///   If the GeoLocationType of this field is an Address, the required fields for this address
    /// </summary>
    public AddressFields? RequiredAddressFields { get; init; }

    internal CreateItemField.Command ToInternal(BonesUser user)
    {
        return new(ProjectId, Name, IsRequired, Type, CanBeNegative, PossibleValues?.ToDictionary(), GeoLocationType, RequiredAddressFields, user);
    }
}