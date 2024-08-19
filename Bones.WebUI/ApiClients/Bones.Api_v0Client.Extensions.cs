using System.Text;

namespace Bones.ApiClients;

public partial class Bones_Api_v0Client
{
    private bool IsLoggedIn { get; set; } = false;

    public async Task<bool> TrySetLoginAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        
        StringActionResult? user = await WhoAmIAsync();
        if (!string.IsNullOrEmpty(user?.Value))
        {
            IsLoggedIn = true;
            return true;
        }
        
        _httpClient.DefaultRequestHeaders.Clear();
        IsLoggedIn = false;
        return false;
    }
    
    // partial void Initialize()
    
    // static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings) 

    // This gets called before the one that ends with string
    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
    {
        throw new NotImplementedException();
    }
    
    // This gets called after the one that ends in string builder
    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        throw new NotImplementedException();
    }
    
    partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
    {
        throw new NotImplementedException();
    }
}