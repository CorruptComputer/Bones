namespace Bones.Shared.Backend.Enums;

/// <summary>
///   The uses for item layouts
/// </summary>
[Flags]
public enum ItemLayoutUses : ushort
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