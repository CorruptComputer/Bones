namespace Bones.Shared.Enums;

/// <summary>
///   The use for an item layout
/// </summary>
public enum ItemLayoutUse : ushort
{
    /// <summary>
    ///   None
    /// </summary>
    None = 0,

    /// <summary>
    ///   They can be used in work items
    /// </summary>
    WorkItems = 1,

    /// <summary>
    ///   They can be used in assets
    /// </summary>
    Assets = 2
}