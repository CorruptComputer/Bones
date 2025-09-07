using System.Net;
using Bones.WebUI.Services.Singleton;
using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Pages.Account;

/// <summary>
///   Page for changing the user's email address.
/// </summary>
/// <param name="apiClient"></param>
/// <param name="navManager"></param>
/// <param name="authStateProvider"></param>
public partial class ChangeEmailPage(BonesApiClient apiClient, NavigationManager navManager, BonesAuthenticationStateProvider authStateProvider) : ComponentBase
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
            ValidationErrors = [.. ValidationErrors, "New email cannot be empty."];
        }

        if (string.IsNullOrWhiteSpace(NewEmailAgain))
        {
            ValidationErrors = [.. ValidationErrors, "Please confirm your new email."];
        }

        if (NewEmail != NewEmailAgain)
        {
            ValidationErrors = [.. ValidationErrors, "New email and confirmation do not match."];
        }

        if (FormValid && ValidationErrors.Length > 0)
        {
            FormValid = false;

            return;
        }

        try
        {
            await apiClient.Account.My.Email.PutAsync(new()
            {
                NewEmail = NewEmail
            });
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.BadRequest)
        {
            ValidationErrors = [];
            foreach (string key in ex.Data.Keys)
            {
                ValidationErrors = [.. ValidationErrors, $"{key}: {string.Join('\n', ex.Data[key])}"];
            }
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

        await authStateProvider.ClearCurrentUserInBrowserStorageAsync(CancellationToken.None);
        navManager.NavigateTo("/");
    }
}