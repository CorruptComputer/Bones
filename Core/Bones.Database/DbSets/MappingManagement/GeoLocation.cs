using Bones.Database.DbConsts;
using Bones.Database.DbSets.ProjectManagement;
using GeoJSON.Text.Feature;

namespace Bones.Database.DbSets.MappingManagement;

/// <summary>
///     Model for the MappingManagement.GeoLocations table
/// </summary>
[Table(TableNames.MappingManagement.GeoLocations, Schema = SchemaNames.MappingManagement)]
[PrimaryKey(nameof(Id))]
public sealed class GeoLocation
{
    /// <summary>
    ///     Internal ID for the GeoLocation
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The ID of the project this GeoLocation belongs to
    /// </summary>
    [ForeignKey(nameof(Project))]
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   The latitude of this object, centeroid if its a polygon
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    ///   The longitude of this object, centeroid if its a polygon
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    ///   The geometry of this object
    /// </summary>
    public FeatureCollection? Geometry { get; set; }

    #region OsmObject
    /// <summary>
    ///   The OSM object this GeoLocation is based on
    /// </summary>
    public OsmObject? OsmObject { get; set; }

    /// <summary>
    ///   The last acknowledged OSM version, once acknowledged the geometry from that version will be copied to here
    /// </summary>
    public long? LastAcknowledgedOsmVersion { get; set; }
    #endregion
    
    #region Address
    /// <summary>
    ///   The street number of this address
    /// </summary>
    public string? StreetNumber { get; set; }

    /// <summary>
    ///   The name of the street, including the street type (e.g. "Road" or "Avenue")
    /// </summary>
    public string? StreetName { get; set; }

    /// <summary>
    ///   The name of the city or place
    /// </summary>
    public string? CityOrPlace { get; set; }

    /// <summary>
    ///   The name of the state or province
    /// </summary>
    public string? StateOrProvince { get; set; }

    /// <summary>
    ///   The postal/zip code
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    ///   The name of the county
    /// </summary>
    public string? County { get; set; }

    /// <summary>
    ///   The name of the country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    ///   The status of the geolocation for this address, null if not attempted, true if successful, and false if failed.
    ///   Geolocating the address will set the Latitude and Longitude fields.
    /// </summary>
    public bool? GeoLocated { get; set; }
    #endregion
    
    /// <summary>
    ///   Disables viewing of this,
    ///   and when all references to it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}
