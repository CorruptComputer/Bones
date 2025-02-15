using Bones.Database.DbConsts;
using Bones.Shared.Backend.Enums;
using GeoJSON.Text.Feature;

namespace Bones.Database.DbSets.MappingManagement;

/// <summary>
///     Model for the MappingManagement.OsmObjects table<br /><br />
///     OSM objects should be completely agnostic to projects, any project can reference and reuse this same object.
///     The OSM object should be converted to GeoJSON before being stored here.
/// </summary>
[Table(TableNames.MappingManagement.OsmObjects, Schema = SchemaNames.MappingManagement)]
[PrimaryKey(nameof(Id))]
public class OsmObject
{
    /// <summary>
    ///     Internal ID for the OsmObject
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The type of OSM object
    /// </summary>
    public required OsmType OsmType { get; init; }

    /// <summary>
    ///   The ID of the OSM object
    /// </summary>
    public required long OsmId { get; init; }

    /// <summary>
    ///   The last version of the OSM object, null if it has never been pulled
    /// </summary>
    public long? LastOsmVersion { get; set; }

    /// <summary>
    ///   The last time the geometry was updated from OSM, null if it has never been pulled
    /// </summary>
    public DateTimeOffset? LastOsmGeometryUpdate { get; set; }

    /// <summary>
    ///   The geometry of this object, null if it has never been pulled
    /// </summary>
    public Feature? OsmGeometry { get; set; }

    /// <summary>
    ///   Signals that this object has been deleted on OSM, this will disable updates to the geometry for this and freeze it at the last known version.
    /// </summary>
    public bool DeletedOnOsmFlag { get; set; } = false;

    /// <summary>
    ///   Disables viewing of this,
    ///   and when all references to it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}
