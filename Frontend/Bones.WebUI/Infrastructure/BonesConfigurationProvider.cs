namespace Bones.WebUI.Infrastructure;

/// <summary>
///   Handles storing and retrieving the configuration for the web UI.
/// </summary>
/// <param name="sessionStorageService"></param>
/// <param name="apiClient"></param>
public class BonesConfigurationProvider(SessionStorageService sessionStorageService, BonesApiClient apiClient)
{
    /// <summary>
    ///   The sessionStorage key for the web UI configuration
    /// </summary>
    public const string WEB_UI_CONFIG_KEY = "WebUiConfig";

    /// <summary>
    ///   Gets the web UI configuration from sessionStorage
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<WebUiConfigResponse> GetWebUiConfigAsync(CancellationToken cancellationToken)
    {
        WebUiConfigResponse? config = await sessionStorageService.GetItemAsync<WebUiConfigResponse>(WEB_UI_CONFIG_KEY, cancellationToken);

        if (config == null)
        {
            config = await apiClient.GetWebConfigAsync(cancellationToken);

            await sessionStorageService.SetItemAsync(WEB_UI_CONFIG_KEY, config, cancellationToken);
        }

        return config;
    }
}
