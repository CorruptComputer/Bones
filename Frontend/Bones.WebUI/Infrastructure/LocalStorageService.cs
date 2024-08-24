using System.Text.Json;
using Microsoft.JSInterop;

namespace Bones.WebUI.Infrastructure;

public sealed class LocalStorageService(IJSRuntime jsRuntime)
{
    public const string CURRENT_USER_KEY = "InfoResponse";

    public async Task<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken)
    {
        string? json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, key);

        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItemAsync<T>(string key, T value, CancellationToken cancellationToken)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, JsonSerializer.Serialize(value));
    }

    public async Task RemoveItemAsync(string key, CancellationToken cancellationToken)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", cancellationToken, key);
    }
}