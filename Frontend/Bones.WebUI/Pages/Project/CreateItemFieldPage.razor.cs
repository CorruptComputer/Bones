using Bones.Api.Client;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Create Item Field page
/// </summary>
public partial class CreateItemFieldPage(BonesApiClient ApiClient, NavigationManager NavManager, ILogger<CreateItemFieldPage> Logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to create this item field in
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

    /// <summary>
    ///   Did the request to the API result in an error?
    /// </summary>
    public bool ApiError { get; set; } = false;

    /// <summary>
    ///   Is the form valid?
    /// </summary>
    public bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users inputs
    /// </summary>
    public string[] ValidationErrors { get; set; } = [];

    private string FieldName { get; set; } = string.Empty;
    private Api.Client.FieldType FieldType { get; set; } = Api.Client.FieldType.Text;
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
    ///   Send the request to create to the API, if it errors tell the user what went wrong.
    /// </summary>
    public async Task SendCreateRequestAsync()
    {
        if (!FormValid)
        {
            return;
        }

        try
        {
            ApiError = false;

            CreateItemFieldRequest request = GetRequest();
            await ApiClient.CreateItemFieldAsync(ProjectId, request);

            NavManager.NavigateTo(FrontEndUrls.Project.MODIFY_PROJECT.Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, ProjectId.ToString()));
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "Error while creating item field");
            ApiError = true;
        }
    }

    private CreateItemFieldRequest GetRequest()
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
            request.PossibleValues = new();
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