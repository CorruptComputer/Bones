using System;

namespace Bones.WebUI.SubtleCrypto.Models;

/// <summary>
///   Result of a decryption operation
/// </summary>
public record DecryptionResult : ResultBase
{
    /// <summary>
    ///   The decrypted plaintext
    /// </summary>
    public string? PlainText { get; set; }
}
