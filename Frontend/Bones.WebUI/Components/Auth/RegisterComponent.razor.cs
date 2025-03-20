using System.ComponentModel.DataAnnotations;
using Bones.Shared;

namespace Bones.WebUI.Components.Auth;

/// <summary>
///   The page for registering user accounts 
/// </summary>
public partial class RegisterComponent(BonesApiClient ApiClient) : ComponentBase
{
    private bool RegistrationSuccess { get; set; } = false;

    private bool RegistrationApiError { get; set; } = false;

    private RegisterFormModel RegisterForm { get; set; } = new();

    private async Task DoRegistrationAsync()
    {
        try
        {
            RegistrationApiError = false;

            await ApiClient.RegisterAsync(new()
            {
                Email = RegisterForm.Email,
                Password = RegisterForm.Password
            });

            RegistrationSuccess = true;
        }
        catch
        {
            RegistrationSuccess = false;
            RegistrationApiError = true;
        }
    }

    private sealed class RegisterFormModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(StandardRegexes.VALID_PASSWORD, ErrorMessage = "Password must contain at least 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special character")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string PasswordAgain { get; set; } = string.Empty;
    }
}