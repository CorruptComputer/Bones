using System.Diagnostics.CodeAnalysis;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Presets;

internal record PresetFieldInfo
{
    internal required string Name { get; init; }
    internal required bool IsRequired { get; init; }
    internal required FieldType Type { get; init; }
    internal bool? CanBeNegative { get; init; }
    internal Dictionary<string, StringValueMatchingType>? PossibleValues { get; init; }
    internal GeoLocationType? GeoLocationType { get; init; }
    internal AddressFields? RequiredAddressFields { get; init; }

    [MemberNotNullWhen(true, nameof(FieldId))]
    internal bool Created => FieldId.HasValue;

    /// <summary>
    ///   After the field is created, this will be set to the ID of the field
    /// </summary>
    internal Guid? FieldId { get; set; }
}