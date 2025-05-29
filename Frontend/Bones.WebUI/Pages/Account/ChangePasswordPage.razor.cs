using System.Net;
using Bones.Shared.Consts;
using Bones.WebUI.Consts;
using Bones.WebUI.Services.Singleton;

namespace Bones.WebUI.Pages.Account;

/// <summary>
///   
/// </summary>
/// <param name="apiClient"></param>
/// <param name="navManager"></param>
/// <param name="localStorageService"></param>
public partial class ChangePasswordPage(BonesApiClient apiClient, NavigationManager navManager, LocalStorageService localStorageService) : ComponentBase
{
    private bool FormValid { get; set; }

    private string[] ValidationErrors { get; set; } = [];

    private string CurrentPassword { get; set; } = string.Empty;

    private string NewPassword { get; set; } = string.Empty;

    private string NewPasswordAgain { get; set; } = string.Empty;

    private bool InvalidateOtherSessions { get; set; } = false;

    private async Task ChangePasswordAsync()
    {
        if (FormValid && ValidationErrors.Length > 0)
        {
            FormValid = false;

            return;
        }

        try
        {
            Guid currentSessionId = await localStorageService.GetItemAsync<Guid>(LocalStorageConsts.SESSION_ID_KEY, string.Empty, CancellationToken.None);

            await apiClient.ChangeMyPasswordAsync(new()
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                InvalidateOtherSessions = InvalidateOtherSessions,
                CurrentSessionId = InvalidateOtherSessions ? currentSessionId : null
            });
        }
        catch (ApiException<ErrorResponse> ex) when (ex.StatusCode == (int)HttpStatusCode.BadRequest)
        {
            // This is a validation error from the API
            ValidationErrors = [.. ex.Result.Errors.Select(e => $"{e.Key}: {string.Join('\n', e.Value)}")];
            FormValid = false;

            return;
        }
        catch (Exception ex)
        {
            ValidationErrors = [ex.Message];
            FormValid = false;

            return;
        }

        if (!FormValid || ValidationErrors.Length > 0)
        {
            FormValid = false;

            return;
        }

        navManager.NavigateTo(FrontEndUrls.Account.MY_PROFILE);
    }
}