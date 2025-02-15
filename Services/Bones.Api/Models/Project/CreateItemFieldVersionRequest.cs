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

    internal CreateItemFieldVersionCommand ToInternal(Guid itemFieldId, BonesUser user)
    {
        return new(itemFieldId, Name, IsRequired, Type, CanBeNegative, PossibleValues, GeoLocationType, RequiredAddressFields, user);
    }
}