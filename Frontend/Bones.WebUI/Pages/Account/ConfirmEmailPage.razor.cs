using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Account;

public partial class ConfirmEmailPage
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string? UserId { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? Code { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? ChangedEmail { get; set; }

    public enum State
    {
        Unknown,
        Success,
        Failure
    }

    public State CurrentState { get; set; } = State.Unknown;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(UserId)
            || !Guid.TryParse(UserId, out Guid parsedUserId)
            || string.IsNullOrEmpty(Code))
        {
            CurrentState = State.Failure;
            return;
        }

        try
        {
            await ApiClient.ConfirmEmailAsync(parsedUserId, Code, ChangedEmail);
            CurrentState = State.Success;
        }
        catch (Exception)
        {
            CurrentState = State.Failure;
        }
    }
}