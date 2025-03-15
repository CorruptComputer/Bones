using System.Text.Json;
using Bones.Shared;
using Bones.WebUI.SubtleCrypto;
using Bones.WebUI.SubtleCrypto.Models;
using Microsoft.JSInterop;

namespace Bones.WebUI.Infrastructure;

/// <summary>
///   Handles all the info in localStorage (semi-persistent)
/// </summary>
/// <param name="jsRuntime"></param>
public sealed class LocalStorageService(IJSRuntime jsRuntime)
{
    /// <summary>
    ///   The localStorage key for the current users basic info
    /// </summary>
    public const string CURRENT_USER_KEY = "MyBasicInfoResponse";

    /// <summary>
    ///   The localStorage key for the current users session id
    /// </summary>
    public const string SESSION_ID_KEY = "SessionId";

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
        // This is the only one not encrypted, needs to be read to get the encryption key to read the rest
        if (key == SESSION_ID_KEY) 
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

        EncryptionResult? encrypted = JsonSerializer.Deserialize<EncryptionResult>(encryptedJson);
        
        if (encrypted == null || !encrypted.Succeeded
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
        if (key == SESSION_ID_KEY) 
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

        await jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, JsonSerializer.Serialize(encryptedValue));
        return true;
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