using System.ComponentModel.DataAnnotations;

namespace Bones.WebUI.Components.SystemAdmin.SystemSettings;

/// <summary>
///   Component for viewing and updating the SMTP configuration
/// </summary>
/// <param name="apiClient"></param>
public partial class SmtpConfigurationComponent(BonesApiClient apiClient) : ComponentBase
{
    private bool ApiError { get; set; } = false;

    private SmtpConfigurationFormModel Model { get; set; } = new();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await FetchFromAPI();
        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await FetchFromAPI();
        await base.OnParametersSetAsync();
    }

    private async Task FetchFromAPI()
    {
        ApiError = false;

        try
        {
            GetSmtpConfigResponse? smtpConfig = await apiClient.SysAdmin.Settings.SmtpConfig.GetAsync();

            if (smtpConfig is null)
            {
                ApiError = true;
                return;
            }

            Model = new()
            {
                IsEnabled = smtpConfig.IsEnabled,
                Server = smtpConfig.Server,
                Port = smtpConfig.Port.ToString(),
                UseSsl = smtpConfig.UseSsl,
                Username = smtpConfig.Username,
                Password = smtpConfig.Password,
                FromAddress = smtpConfig.FromAddress,
                FromName = smtpConfig.FromName
            };
        }
        catch
        {
            ApiError = true;
        }
    }

    private async Task Update()
    {
        ApiError = false;

        try
        {
            await apiClient.SysAdmin.Settings.SmtpConfig.PostAsync(new()
            {
                IsEnabled = Model.IsEnabled,
                Server = Model.Server ?? string.Empty,
                Port = ushort.Parse(Model.Port ?? "25"),
                UseSsl = Model.UseSsl,
                Username = Model.Username ?? string.Empty,
                Password = Model.Password ?? string.Empty,
                FromAddress = Model.FromAddress ?? string.Empty,
                FromName = Model.FromName ?? string.Empty,
                ChangeReason = Model.ChangeReason ?? string.Empty
            });
        }
        catch
        {
            ApiError = true;
        }
    }

    private class SmtpConfigurationFormModel
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "Server is required!")]
        public string? Server { get; set; }

        [Required(ErrorMessage = "Port is required!")]
        public string? Port { get; set; }

        public bool UseSsl { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? FromAddress { get; set; }

        public string? FromName { get; set; }

        [Required(ErrorMessage = "Change reason is required!")]
        public string? ChangeReason { get; set; }
    }
}
