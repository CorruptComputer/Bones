using System.Net;
using Bones.Shared.Consts;
using Bones.Shared.Extensions;

namespace Bones.WebUI.Pages.Account;

/// <summary>
///   
/// </summary>
/// <param name="apiClient"></param>
/// <param name="navManager"></param>
public partial class ChangeEmailPage(BonesApiClient apiClient, NavigationManager navManager) : ComponentBase
{
    private bool FormValid { get; set; }

    private string[] ValidationErrors { get; set; } = [];

    private string NewEmail { get; set; } = string.Empty;

    private string NewEmailAgain { get; set; } = string.Empty;

    private async Task ChangeEmailAsync()
    {
        ValidationErrors = [];

        if (string.IsNullOrWhiteSpace(NewEmail))
        {
            ValidationErrors = [..ValidationErrors, "New email cannot be empty."];
        }

        if (string.IsNullOrWhiteSpace(NewEmailAgain))
        {
            ValidationErrors = [..ValidationErrors, "Please confirm your new email."];
        }

        if (NewEmail != NewEmailAgain)
        {
            ValidationErrors = [..ValidationErrors, "New email and confirmation do not match."];
        }

        if (FormValid && ValidationErrors.Length > 0)
        {
            FormValid = false;

            return;
        }

        try
        {
            await apiClient.ChangeMyEmailAsync(new()
            {
                NewEmail = NewEmail
            });
        }
        catch (ApiException<ErrorResponse> ex) when (ex.StatusCode == (int)HttpStatusCode.BadRequest)
        {
            // This is a validation error from the API
            ValidationErrors = [..ex.Result.Errors.Select(e => $"{e.Key}: {string.Join('\n', e.Value)}")];
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

        await apiClient.LogoutAsync();
        navManager.NavigateTo("/");
    }
}