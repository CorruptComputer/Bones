using Bones.Shared.Consts;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Item Field page
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
    private FieldType FieldType { get; set; } = Api.Client.FieldType.TextField;
    private bool IsRequired { get; set; } = false;

    private bool CanBeNegative { get; set; } = false;

    private GeoLocationType GeoLocationType { get; set; } = GeoLocationType.OsmObject;
    private bool StreetNumberRequired { get; set; } = false;
    private bool StreetNameRequired { get; set; } = false;
    private bool CityOrPlaceRequired { get; set; } = false;
    private bool StateOrProvinceRequired { get; set; } = false;
    private bool PostalCodeRequired { get; set; } = false;
    private bool CountyRequired { get; set; } = false;
    private bool CountryRequired { get; set; } = false;

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
        if (ItemFieldId == null)
        {
            return;
        }

        GetLatestItemFieldVersionResponse latestVersion = await ApiClient.GetLatestItemFieldVersionAsync(ItemFieldId.Value);
        FieldName = latestVersion.Name;
        FieldType = latestVersion.Type;
        IsRequired = latestVersion.IsRequired;

        CanBeNegative = latestVersion.CanBeNegative ?? false;

        GeoLocationType = latestVersion.GeoLocationType ?? GeoLocationType.OsmObject;
        StreetNumberRequired = latestVersion.RequiredAddressFields?.HasFlag(AddressFields.StreetNumber) ?? false;
        StreetNameRequired = latestVersion.RequiredAddressFields?.HasFlag(AddressFields.StreetName) ?? false;
        CityOrPlaceRequired = latestVersion.RequiredAddressFields?.HasFlag(AddressFields.CityOrPlace) ?? false;
        StateOrProvinceRequired = latestVersion.RequiredAddressFields?.HasFlag(AddressFields.StateOrProvince) ?? false;
        PostalCodeRequired = latestVersion.RequiredAddressFields?.HasFlag(AddressFields.PostalCode) ?? false;
        CountyRequired = latestVersion.RequiredAddressFields?.HasFlag(AddressFields.County) ?? false;
        CountryRequired = latestVersion.RequiredAddressFields?.HasFlag(AddressFields.Country) ?? false;
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
                await ApiClient.CreateItemFieldAsync(ProjectId, request);
            }
            else
            {
                CreateItemFieldVersionRequest request = GetNewVersionRequest();
                await ApiClient.CreateItemFieldVersionAsync(ProjectId, ItemFieldId.Value, request);
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
            Name = FieldName,
            IsRequired = IsRequired,
            Type = FieldType,
        };

        if (request.Type is Api.Client.FieldType.ValueList)
        {
            // TODO: Implement this
            request.PossibleValues = [];
        }
        else if (request.Type is Api.Client.FieldType.Integer or Api.Client.FieldType.Decimal)
        {
            request.CanBeNegative = CanBeNegative;
        }
        else if (request.Type is Api.Client.FieldType.GeoLocation)
        {
            request.GeoLocationType = GeoLocationType;

            if (request.GeoLocationType is Api.Client.GeoLocationType.Address)
            {
                request.RequiredAddressFields = AddressFields.None;

                if (StreetNumberRequired)
                {
                    request.RequiredAddressFields |= AddressFields.StreetNumber;
                }

                if (StreetNameRequired)
                {
                    request.RequiredAddressFields |= AddressFields.StreetName;
                }

                if (CityOrPlaceRequired)
                {
                    request.RequiredAddressFields |= AddressFields.CityOrPlace;
                }

                if (StateOrProvinceRequired)
                {
                    request.RequiredAddressFields |= AddressFields.StateOrProvince;
                }

                if (PostalCodeRequired)
                {
                    request.RequiredAddressFields |= AddressFields.PostalCode;
                }

                if (CountyRequired)
                {
                    request.RequiredAddressFields |= AddressFields.County;
                }

                if (CountryRequired)
                {
                    request.RequiredAddressFields |= AddressFields.Country;
                }
            }
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

        if (request.Type is Api.Client.FieldType.ValueList)
        {
            // TODO: Implement this
            request.PossibleValues = [];
        }
        else if (request.Type is Api.Client.FieldType.Integer or Api.Client.FieldType.Decimal)
        {
            request.CanBeNegative = CanBeNegative;
        }
        else if (request.Type is Api.Client.FieldType.GeoLocation)
        {
            request.GeoLocationType = GeoLocationType;

            if (request.GeoLocationType is Api.Client.GeoLocationType.Address)
            {
                request.RequiredAddressFields = AddressFields.None;

                if (StreetNumberRequired)
                {
                    request.RequiredAddressFields |= AddressFields.StreetNumber;
                }

                if (StreetNameRequired)
                {
                    request.RequiredAddressFields |= AddressFields.StreetName;
                }

                if (CityOrPlaceRequired)
                {
                    request.RequiredAddressFields |= AddressFields.CityOrPlace;
                }

                if (StateOrProvinceRequired)
                {
                    request.RequiredAddressFields |= AddressFields.StateOrProvince;
                }

                if (PostalCodeRequired)
                {
                    request.RequiredAddressFields |= AddressFields.PostalCode;
                }

                if (CountyRequired)
                {
                    request.RequiredAddressFields |= AddressFields.County;
                }

                if (CountryRequired)
                {
                    request.RequiredAddressFields |= AddressFields.Country;
                }
            }
        }

        return request;
    }
}