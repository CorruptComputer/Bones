using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///  item Field page
/// </summary>
public partial class ItemFieldPage(BonesApiClient ApiClient, NavigationManager NavManager, ILogger<ItemFieldPage> Logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the project the parent item field belongs to
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

    /// <summary>
    ///   The ID of the item field to load on this page
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "itemFieldId")]
    public Guid? ItemFieldId { get; set; }

    private string TitleText => ItemFieldId.HasValue ? "Edit Item Field" : "Create Item Field";

    private string SaveButtonText => ItemFieldId.HasValue ? "Save" : "Create";

    private bool ApiError { get; set; } = false;

    private bool FormValid { get; set; } = false;

    private string[] ValidationErrors { get; set; } = [];

    private string FieldName { get; set; } = string.Empty;
    private FieldType FieldType { get; set; } = FieldType.TextField;
    private bool IsRequired { get; set; } = false;

    private bool CanBeNegative { get; set; } = false;

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
        if (ItemFieldId is null)
        {
            return;
        }

        GetLatestItemFieldVersionResponse? latestVersion = await ApiClient.ItemLayout.Fields[ItemFieldId.Value].Latest.GetAsync();
        if (latestVersion is null)
        {
            ApiError = true;
            return;
        }
        FieldName = latestVersion.Name;
        FieldType = latestVersion.Type ?? FieldType.TextField;
        IsRequired = latestVersion.IsRequired;

        CanBeNegative = latestVersion.CanBeNegative ?? false;
    }

    private async Task SendCreateRequestAsync()
    {
        if (!FormValid)
        {
            return;
        }

        try
        {
            ApiError = false;

            if (ItemFieldId == null)
            {
                CreateItemFieldRequest request = GetNewFieldRequest();
                await ApiClient.ItemLayout.Fields.Create.PostAsync(request);
            }
            else
            {
                CreateItemFieldVersionRequest request = GetNewVersionRequest();
                await ApiClient.ItemLayout.Fields[ItemFieldId.Value].PostAsync(request);
            }



            NavManager.NavigateTo(FrontEndUrls.Project.MODIFY_PROJECT.Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, ProjectId.ToString()));
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "Error while creating initiative");
            ApiError = true;
        }
    }

    private CreateItemFieldRequest GetNewFieldRequest()
    {
        CreateItemFieldRequest request = new()
        {
            ProjectId = ProjectId,
            Name = FieldName,
            IsRequired = IsRequired,
            Type = FieldType,
        };

        if (request.Type is FieldType.ValueList)
        {
            // TODO: Implement this
            request.PossibleValues = [];
        }
        else if (request.Type is FieldType.Integer or FieldType.Decimal)
        {
            request.CanBeNegative = CanBeNegative;
        }

        return request;
    }

    private CreateItemFieldVersionRequest GetNewVersionRequest()
    {
        CreateItemFieldVersionRequest request = new()
        {
            Name = FieldName,
            IsRequired = IsRequired,
            Type = FieldType,
        };

        if (request.Type is FieldType.ValueList)
        {
            // TODO: Implement this
            request.PossibleValues = [];
        }
        else if (request.Type is FieldType.Integer or FieldType.Decimal)
        {
            request.CanBeNegative = CanBeNegative;
        }

        return request;
    }
}