namespace Bones.Database.Models;

/// <summary>
///   The configuration for the database
/// </summary>
public sealed record DatabaseConfiguration
{
    /// <summary>
    ///   Connection string to use with the Postgres DB
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    ///   Use an in-memory DB instead of Postgres
    /// </summary>
    public bool? UseInMemoryDb { get; set; }
}