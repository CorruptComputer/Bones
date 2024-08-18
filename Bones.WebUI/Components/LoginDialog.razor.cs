using System.Text.RegularExpressions;
using Bones.ApiClients;

namespace Bones.WebUI.Components;

public partial class LoginDialog(Bones_Api_v0Client apiClient)
{
    private bool LoginFlow { get; set; } = true;
    
    private bool RegistrationSuccess { get; set; } = false;
    private string RegistrationError { get; set; } = string.Empty;
    
    private string EmailAddress { get; set; } = string.Empty;
    
    private string Password { get; set; } = string.Empty;
    private string PasswordAgain { get; set; } = string.Empty;

    private bool PasswordComplexEnough { get; set; } = false;
    private bool PasswordsMatch { get; set; } = true;
    
    public void PasswordChangedEvent(string newText)
    {
        Password = newText;

        if (LoginFlow)
        {
            return;
        }

        bool hasNumber = PasswordContainsNumber().IsMatch(Password);
        bool hasLower = PasswordContainsLower().IsMatch(Password);
        bool hasUpper = PasswordContainsUpper().IsMatch(Password);
        bool hasSpecial = PasswordContainsSpecial().IsMatch(Password);
        bool properLength = Password.Length >= 8;

        if (!hasNumber || !hasLower || !hasUpper || !hasSpecial || !properLength)
        {
            PasswordComplexEnough = false;
        }
        else
        {
            PasswordComplexEnough = true;
        }
        
        PasswordsMatch = Password == PasswordAgain;
    }
    
    public void PasswordAgainChangedEvent(string newText)
    {
        PasswordAgain = newText;
        PasswordsMatch = Password == PasswordAgain;
    }

    public async Task DoRegistrationAsync()
    {
        // TODO:
        //   - Send this to the API
        //   - API should reply back with a user ID if successful
        //   - Tell the user to check their email for the confirmation link
        if (PasswordComplexEnough && PasswordsMatch)
        {
            try
            {
                RegistrationError = string.Empty;
                await apiClient.CreateAccountAsync(new()
                {
                    Email = EmailAddress,
                    Password = Password
                });

                RegistrationSuccess = true;
            }
            catch (ApiException<string> ex)
            {
                RegistrationSuccess = false;
                RegistrationError = ex.Result;
            }
        }
        
    }
    
    [GeneratedRegex(@"[0-9]")]
    private static partial Regex PasswordContainsNumber();
        
    [GeneratedRegex(@"[a-z]")]
    private static partial Regex PasswordContainsLower();
        
    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex PasswordContainsUpper();

    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    private static partial Regex PasswordContainsSpecial();
    
    public void DoLogin()
    {
        // TODO:
        //   - Send this to the API
        //   - API should reply back with a token if successful
        //   - Set the token in the users session somehow and use that for all future API calls
        //   - Redirect back to home page
        
    }

    public void GoToRegistration()
    {
        LoginFlow = false;
    }
}