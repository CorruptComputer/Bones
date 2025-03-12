using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.GenericItems;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.GenericItem;

/// <summary>
///   API response for the GetLatestItemLayoutVersionFieldsAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetLatestItemLayoutVersionFieldsResponse))]
public sealed record GetLatestItemLayoutVersionFieldsResponse
{
    /// <summary>
    ///     ID for the ItemFieldVersion
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///   The order number for which this field should be displayed
    /// </summary>
    public required uint OrderNumber { get; init; }

    /// <summary>
    ///   The name of this ItemFieldVersion
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The FieldType for this field
    /// </summary>
    public required FieldType Type { get; set; }

    /// <summary>
    ///   Is this field required to have a value?
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    ///   If the Type of this field is either an Integer or Decimal, can it be negative?
    /// </summary>
    public bool? CanBeNegative { get; set; }

    internal static List<GetLatestItemLayoutVersionFieldsResponse> FromInternal(GenericItemLayout layout)
    {
        return layout.CurrentVersion?.FieldLinks.Select(fl => new GetLatestItemLayoutVersionFieldsResponse
        {
            Id = fl.FieldVersion.Id,
            OrderNumber = fl.OrderNumber,
            Name = fl.FieldVersion.Name,
            Type = fl.FieldVersion.Type,
            IsRequired = fl.FieldVersion.IsRequired,
            CanBeNegative = fl.FieldVersion.CanBeNegative
        }).ToList() ?? [];
    }
}