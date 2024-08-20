using System.Text;

namespace Bones.WebUI.ApiClients;

public partial class BonesApiClient
{
    private bool IsLoggedIn { get; set; } = false;

    public async Task<bool> TrySetLoginAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
      
        GetUserInfoResponseOk user = await GetInfoAsync();
        if (!string.IsNullOrEmpty(user.Value.Email))
        {
            IsLoggedIn = true;
            return true;
        }
      
        _httpClient.DefaultRequestHeaders.Authorization = null;
        IsLoggedIn = false;
        return false;
    }

    partial void Initialize()
    {
        //_httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin","*");
        //_httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Headers","Origin, X-Requested-With, Content-Type, Accept");
    }
    
    // static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings) 

    // This gets called before the one that ends with string
    //partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
    //{
    //    throw new NotImplementedException();
    //}
    
    // This gets called after the one that ends in string builder
    //partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    //{
    //    throw new NotImplementedException();
    //}
    
    //partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
    //{
    //    throw new NotImplementedException();
    //}
}