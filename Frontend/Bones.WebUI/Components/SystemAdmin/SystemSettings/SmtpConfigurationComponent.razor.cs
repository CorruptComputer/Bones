namespace Bones.WebUI.Components.SystemAdmin.SystemSettings;

/// <summary>
///   Component for viewing and updating the SMTP configuration
/// </summary>
/// <param name="apiClient"></param>
public partial class SmtpConfigurationComponent(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   Is the form valid?
    /// </summary>
    protected bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users input
    /// </summary>
    protected string[] ValidationErrors { get; set; } = [];

    /// <summary>
    ///   Is SMTP enabled?
    /// </summary>
    public bool? IsEnabled { get; set; }

    /// <summary>
    ///   The SMTP server to use for sending emails
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    ///   The port to use for the SMTP server
    /// </summary>
    public string? Port { get; set; }

    /// <summary>
    ///   Whether to use SSL for the connection
    /// </summary>
    public bool? UseSsl { get; set; }

    /// <summary>
    ///   The username to use for the SMTP server
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///   The password to use for the SMTP server
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///   The email address to use as the from address for emails sent by this system
    /// </summary>
    public string? FromAddress { get; set; }

    /// <summary>
    ///   The name to use as the from name for emails sent by this system
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    ///   The reason for the change
    /// </summary>
    public string? ChangeReason { get; set; }

    /// <summary>
    ///   Fires when the page is loaded
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await FetchFromAPI();

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///   Fires if the same page but with a different parameter is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await FetchFromAPI();

        await base.OnParametersSetAsync();
    }

    private async Task FetchFromAPI()
    {
        GetSmtpConfigResponse smtpConfig = await apiClient.GetSmtpConfigAsync();

        IsEnabled = smtpConfig.IsEnabled;
        Server = smtpConfig.Server;
        Port = smtpConfig.Port.ToString();
        UseSsl = smtpConfig.UseSsl;
        Username = smtpConfig.Username;
        Password = smtpConfig.Password;
        FromAddress = smtpConfig.FromAddress;
        FromName = smtpConfig.FromName;
    }

    /// <summary>
    ///   Sends the request to update the background service user configuration
    /// </summary>
    /// <returns></returns>
    protected async Task Update()
    {
        if (!FormValid)
        {
            return;
        }

        try
        {
            await apiClient.SaveSmtpConfigAsync(new()
            {
                IsEnabled = IsEnabled ?? false,
                Server = Server,
                Port = ushort.Parse(Port ?? "25"),
                UseSsl = UseSsl ?? false,
                Username = Username,
                Password = Password,
                FromAddress = FromAddress,
                FromName = FromName,
                ChangeReason = ChangeReason
            });
        }
        catch (ApiException e)
        {
            ValidationErrors = [e.Message];
        }
    }
}
