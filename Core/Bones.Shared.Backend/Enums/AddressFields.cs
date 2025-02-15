namespace Bones.Shared.Backend.Enums;

/// <summary>
///   The fields that can be included in an address
/// </summary>
[Flags]
public enum AddressFields
{
    /// <summary>
    ///   None
    /// </summary>
    None = 0,
    
    /// <summary>
    ///   The numbers at the start of an address
    /// </summary>
    StreetNumber = 1,

    /// <summary>
    ///   The name of the street
    /// </summary>
    StreetName = 2,

    /// <summary>
    ///   The name of the city or place
    /// </summary>
    CityOrPlace = 4,

    /// <summary>
    ///   The name of the state or province
    /// </summary>
    StateOrProvince = 8,

    /// <summary>
    ///   The postal/zip code
    /// </summary>
    PostalCode = 16,

    /// <summary>
    ///   The name of the county
    /// </summary>
    County = 32,

    /// <summary>
    ///   The name of the country
    /// </summary>
    Country = 64,
}
