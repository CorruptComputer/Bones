using Bones.Shared;
using MudBlazor;

namespace Bones.WebUI.Pages.Account;

public partial class RegisterPage
{
    private bool RegistrationSuccess { get; set; } = false;

    public bool RegistrationApiError { get; set; } = false;

    public bool FormValid { get; set; }

    public string[] ValidationErrors { get; set; } = [];

    private MudTextField<string> EmailAddress { get; set; } = new();

    private MudTextField<string> Password { get; set; } = new();
    private MudTextField<string> PasswordAgain { get; set; } = new();

    public async Task DoRegistrationAsync()
    {
        try
        {
            RegistrationApiError = false;

            await ApiClient.RegisterAsync(new()
            {
                Email = EmailAddress.Text,
                Password = Password.Text
            });

            RegistrationSuccess = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while registering user");
            RegistrationSuccess = false;
            RegistrationApiError = true;
        }
    }



    private IEnumerable<string> PasswordStrengthCheck()
    {
        if (string.IsNullOrWhiteSpace(Password.Text))
        {
            yield return "Password is required!";
            yield break;
        }

        if (Password.Text.Length >= 8)
        {
            yield return "Password be at least 8 characters long.";
        }

        if (!StandardRegexes.PasswordContainsUpper().IsMatch(Password.Text))
        {
            yield return "Password must contain at least one capital letter";
        }

        if (!StandardRegexes.PasswordContainsLower().IsMatch(Password.Text))
        {
            yield return "Password must contain at least one lowercase letter";
        }

        if (!StandardRegexes.PasswordContainsNumber().IsMatch(Password.Text))
        {
            yield return "Password must contain at least one digit";
        }

        if (!StandardRegexes.PasswordContainsSpecial().IsMatch(Password.Text))
        {
            yield return "Password must contain at least one special character";
        }
    }

    private string? PasswordMatch()
    {
        if (Password.Text != PasswordAgain.Text)
        {
            return "Passwords don't match";
        }

        return null;
    }
}