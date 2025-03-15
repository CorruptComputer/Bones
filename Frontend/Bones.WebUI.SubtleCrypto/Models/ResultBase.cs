namespace Bones.WebUI.SubtleCrypto.Models;

/// <summary>
///   Base class for all results
/// </summary>
public abstract record ResultBase
{
    /// <summary>
    ///   Whether the operation was successful
    /// </summary>
    public bool Succeeded { get; set; } = true;

    /// <summary>
    ///   If unsuccessful, the error message as to what went wrong
    /// </summary>
    public string? ErrorMessage { get; set; }
}
