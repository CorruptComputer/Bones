using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Auth;

public partial class ConfirmEmail
{
    [Parameter]
    public string? UserId { get; set; }

    [Parameter]
    public string? Code { get; set; }

    [Parameter]
    public string? ChangedEmail { get; set; }

    public enum State
    {
        Confirming,
        Success,
        Failure
    }

    public State CurrentState { get; set; } = State.Confirming;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(UserId)
            || !Guid.TryParse(UserId, out _)
            || string.IsNullOrEmpty(Code))
        {
            CurrentState = State.Failure;
            return;
        }

        try
        {
            await ApiClient.AuthConfirmEmailAsync(UserId, Code, ChangedEmail);
            CurrentState = State.Success;
        }
        catch (Exception)
        {
            CurrentState = State.Failure;
        }
    }
}