using Bones.WebUI.Models.Interop.SubtleCrypto;
using Microsoft.JSInterop;

namespace Bones.WebUI;

/// <summary>
///   Interop class to call the SubtleCrypto encrypt and decrypt functions from Blazor WASM
/// </summary>
public static class SubtleCryptoInterop
{
    private const string _baseObjectContainer = "window.subtleCryptoInterop";

    /// <summary>
    ///   Encrypt the plaintext using the given key.
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="base64Key">An AES key encoded in base64</param>
    /// <param name="plaintext">The text to be encrypted</param>
    /// <returns><see cref="EncryptionResult"/></returns>
    public static async Task<EncryptionResult> EncryptAsync(IJSRuntime jsRuntime, string base64Key, string plaintext)
    {
        EncryptionResult result = new();

        try
        {
            result = await jsRuntime.InvokeAsync<EncryptionResult>($"{_baseObjectContainer}.encrypt", Convert.FromBase64String(base64Key), plaintext);
        }
        catch (Exception err)
        {
            result.Succeeded = false;
            result.ErrorMessage = err.Message;
        }

        return result;
    }

    /// <summary>
    ///   Decrypt the ciphertext using the given key and IV.
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="base64Key">An AES key encoded in base64</param>
    /// <param name="iv">Base64-encoded IV</param>
    /// <param name="ciphertext">Base64-encoded ciphertext</param>
    /// <returns><see cref="DecryptionResult"/></returns>
    public static async Task<DecryptionResult> DecryptAsync(IJSRuntime jsRuntime, string base64Key, string iv, string ciphertext)
    {
        DecryptionResult result = new();

        try
        {
            string jsResult = await jsRuntime.InvokeAsync<string>($"{_baseObjectContainer}.decrypt", Convert.FromBase64String(base64Key), iv, ciphertext);
            result.PlainText = jsResult;
        }
        catch (Exception err)
        {
            result.Succeeded = false;
            result.ErrorMessage = err.Message;
        }

        return result;
    }
}
