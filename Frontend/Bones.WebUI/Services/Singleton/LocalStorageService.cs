using System.Text.Json;
using Bones.WebUI.Consts;
using Bones.WebUI.SubtleCrypto;
using Bones.WebUI.SubtleCrypto.Models;
using Microsoft.JSInterop;

namespace Bones.WebUI.Services.Singleton;

/// <summary>
///   Handles all the info in localStorage (semi-persistent)
/// </summary>
/// <param name="jsRuntime"></param>
public sealed class LocalStorageService(IJSRuntime jsRuntime)
{
    private readonly List<string> UnEncryptedKeys =
    [
        LocalStorageConsts.SESSION_ID_KEY
    ];

    /// <summary>
    ///   Gets the specified value from localStorage
    /// </summary>
    /// <param name="key"></param>
    /// <param name="base64LocalStorageKey"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T?> GetItemAsync<T>(string key, string base64LocalStorageKey, CancellationToken cancellationToken)
    {
        if (UnEncryptedKeys.Contains(key))
        {
            string? sessionId = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, key);
            return sessionId == null
                ? default
                : JsonSerializer.Deserialize<T>(sessionId);
        }

        string? encryptedJson = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, key);
        if (encryptedJson == null)
        {
            return default;
        }

        EncryptionModel? encrypted = JsonSerializer.Deserialize<EncryptionModel>(encryptedJson);
        if (encrypted is null
            || string.IsNullOrEmpty(encrypted.IV)
            || string.IsNullOrEmpty(encrypted.CipherText))
        {
            return default;
        }

        DecryptionResult? decryptionResult = await SubtleCryptoInterop.DecryptAsync(jsRuntime, base64LocalStorageKey, encrypted.IV, encrypted.CipherText);
        if (string.IsNullOrEmpty(decryptionResult?.PlainText))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(decryptionResult.PlainText);
    }

    /// <summary>
    ///   Sets the specified value in localStorage
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="base64LocalStorageKey"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    public async Task<bool> SetItemAsync<T>(string key, T value, string base64LocalStorageKey, CancellationToken cancellationToken)
    {
        // This is the only one not encrypted, needs to be read to get the encryption key to read the rest
        if (key == LocalStorageConsts.SESSION_ID_KEY)
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, JsonSerializer.Serialize(value));
            return true;
        }

        string json = JsonSerializer.Serialize(value);
        EncryptionResult? encryptedValue = await SubtleCryptoInterop.EncryptAsync(jsRuntime, base64LocalStorageKey, json);

        if (encryptedValue == null || !encryptedValue.Succeeded
            || string.IsNullOrEmpty(encryptedValue.IV)
            || string.IsNullOrEmpty(encryptedValue.CipherText))
        {
            return false;
        }

        string encryptedJson = JsonSerializer.Serialize(new EncryptionModel
        {
            IV = encryptedValue.IV,
            CipherText = encryptedValue.CipherText
        });

        await jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, encryptedJson);
        return true;
    }

    private class EncryptionModel
    {
        public required string IV { get; init; }
        public required string CipherText { get; init; }
    }

    /// <summary>
    ///   Removes the specified item from localStorage
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    public async Task RemoveItemAsync(string key, CancellationToken cancellationToken)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", cancellationToken, key);
    }

    /// <summary>
    ///   Clears all items from localStorage
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ClearAsync(CancellationToken cancellationToken)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.clear", cancellationToken);
    }
}