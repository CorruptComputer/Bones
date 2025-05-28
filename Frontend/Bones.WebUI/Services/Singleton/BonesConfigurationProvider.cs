namespace Bones.WebUI.Services.Singleton;

/// <summary>
///   Handles storing and retrieving the configurations for the web UI.
/// </summary>
/// <param name="sessionStorageService"></param>
/// <param name="apiClient"></param>
public class BonesConfigurationProvider(SessionStorageService sessionStorageService, BonesApiClient apiClient)
{
    /// <summary>
    ///   The sessionStorage key for the API configuration
    /// </summary>
    public const string API_CONFIG_KEY = "ApiConfig";

    /// <summary>
    ///   Gets the API configuration from sessionStorage or the API if its not cached locally.
    ///   This API configuration is a temporary solution as the current values are only really there for testing purposes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ApiConfigResponse> GetApiConfigAsync(CancellationToken cancellationToken)
    {
        ApiConfigResponse? config = await sessionStorageService.GetItemAsync<ApiConfigResponse>(API_CONFIG_KEY, cancellationToken);

        if (config == null)
        {
            config = await apiClient.GetApiConfigAsync(cancellationToken);
            await sessionStorageService.SetItemAsync(API_CONFIG_KEY, config, cancellationToken);
        }

        return config;
    }
}
