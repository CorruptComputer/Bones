using System.Text.Json;
using Microsoft.JSInterop;

namespace Bones.WebUI.Infrastructure;

/// <summary>
///   Handles all the info in sessionStorage (deleted when the browser is closed)
/// </summary>
/// <param name="jsRuntime"></param>
public class SessionStorageService(IJSRuntime jsRuntime)
{
    /// <summary>
    ///   The sessionStorage key for localStorage encryption key
    /// </summary>
    public const string BASE64_LOCALSTORAGE_KEY = "Base64LocalStorageKey";

    /// <summary>
    ///   Gets the specified value from sessionStorage
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken)
    {
        string? json = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", cancellationToken, key);

        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    ///   Sets the specified value in sessionStorage
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    public async Task SetItemAsync<T>(string key, T value, CancellationToken cancellationToken)
    {
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", cancellationToken, key, JsonSerializer.Serialize(value));
    }

    /// <summary>
    ///   Removes the specified item from sessionStorage
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    public async Task RemoveItemAsync(string key, CancellationToken cancellationToken)
    {
        await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", cancellationToken, key);
    }

    /// <summary>
    ///   Clears all items from sessionStorage
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ClearAsync(CancellationToken cancellationToken)
    {
        await jsRuntime.InvokeVoidAsync("sessionStorage.clear", cancellationToken);
    }
}
