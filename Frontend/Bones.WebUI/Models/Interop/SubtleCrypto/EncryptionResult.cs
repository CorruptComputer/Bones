namespace Bones.WebUI.Models.Interop.SubtleCrypto;

/// <summary>
///   Result of an encryption operation
/// </summary>
public record EncryptionResult : ResultBase
{
    /// <summary>
    ///   The encrypted ciphertext
    /// </summary>
    public string? CipherText { get; set; }

    /// <summary>
    ///   The initialization vector used
    /// </summary>
    public string? IV { get; set; }
}
