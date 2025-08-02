namespace Bones.WebUI.Models;

/// <summary>
///   A generic model for drop downs, optionally supports a GUID ID
/// </summary>
public sealed record DropDownModel
{
    /// <summary>
    ///   The display string of the option
    /// </summary>
    public required string DisplayStr { get; init; }

    /// <summary>
    ///   The ID of the option
    /// </summary>
    public required Guid? Id { get; init; }

    /// <summary>
    ///   The string representation of the option, which should just be the DisplayStr.
    ///   There should never really be a reason to display anything with the ID.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => DisplayStr;
}
