using System.Text.Json;
using Microsoft.JSInterop;

namespace Bones.WebUI.Infrastructure;

public sealed class LocalStorageService(IJSRuntime jsRuntime)
{
    public const string CURRENT_USER_KEY = "InfoResponse";

    public async Task<T?> GetItemAsync<T>(string key)
    {
        string? json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);

        if (json == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItem<T>(string key, T value)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }

    public async Task RemoveItemAsync(string key)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}