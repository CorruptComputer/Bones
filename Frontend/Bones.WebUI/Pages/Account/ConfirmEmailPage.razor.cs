using Bones.Api.Client;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Account;

/// <summary>
///   Confirms the users email
/// </summary>
public partial class ConfirmEmailPage(BonesApiClient ApiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the user this request is for
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery]
    public string? UserId { get; set; }

    /// <summary>
    ///   The code to validate the change/confirm email request
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery]
    public string? Code { get; set; }

    /// <summary>
    ///   If this was a request to change the users email, what is their new email?
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery]
    public string? ChangedEmail { get; set; }

    /// <summary>
    ///   The current state of this page
    /// </summary>
    public enum State
    {
        /// <summary>
        ///   ¯\_(ツ)_/¯
        /// </summary>
        Unknown,

        /// <summary>
        ///   Good to go
        /// </summary>
        Success,

        /// <summary>
        ///   Something went wrong
        /// </summary>
        Failure
    }

    /// <summary>
    ///   The current state of this page
    /// </summary>
    public State CurrentState { get; set; } = State.Unknown;

    /// <inheritdoc />
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