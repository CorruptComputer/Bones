namespace Bones.Shared.Backend.Enums;

/// <summary>
///   The type of OSM object
/// </summary>
public enum OsmType
{
    /// <summary>
    ///   A Node, basically just a point
    /// </summary>
    Node = 0,

    /// <summary>
    ///   A Way, a line or area
    /// </summary>
    Way = 1,

    /// <summary>
    ///   A Relation, a collection of nodes, ways, or other relations
    /// </summary>
    Relation = 2
}