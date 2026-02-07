using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Item;

/// <summary>
///   API response for the GetLatestItemLayoutVersionFieldsAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetLatestItemLayoutVersionFieldsResponse))]
public sealed record GetLatestItemLayoutVersionFieldsResponse
{
    /// <summary>
    ///   ID for the ItemFieldVersion
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///   The order number for which this field should be displayed
    /// </summary>
    public required int OrderNumber { get; init; }

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
    ///   If the field type is a ValueList, this will contain the possible values for this field.
    /// </summary>
    public IEnumerable<string>? PossibleValues { get; set; }

    /// <summary>
    ///   If the Type of this field is either an Integer or Decimal, can it be negative?
    /// </summary>
    public bool? CanBeNegative { get; set; }

    internal static List<GetLatestItemLayoutVersionFieldsResponse> FromInternal(ItemLayout layout)
    {
        return layout.Current?.ItemLayoutFieldVersionLinks.Select(fl => new GetLatestItemLayoutVersionFieldsResponse
        {
            Id = fl.ItemFieldVersionId,
            OrderNumber = fl.OrderNumber,
            Name = fl.ItemFieldVersion!.Name,
            Type = fl.ItemFieldVersion.Type,
            IsRequired = fl.ItemFieldVersion.IsRequired,
            PossibleValues = fl.ItemFieldVersion.PossibleValues?.Select(pv => pv.Value),
            CanBeNegative = fl.ItemFieldVersion.CanBeNegative
        }).ToList() ?? [];
    }
}