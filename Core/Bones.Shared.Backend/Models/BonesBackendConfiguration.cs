namespace Bones.Shared.Backend.Models;

/// <summary>
///   Shared configuration for the backend of the Bones project.<br />
///   Stored in /etc/bones/backend.json for deployed envs<br />
///   Or appsettings.Development.json for local development
/// </summary>
public class BonesBackendConfiguration
{
    /// <summary>
    ///   The database connection string, null if using an in-memory database
    /// </summary>
    public string? DatabaseConnectionString { get; set; }

    /// <summary>
    ///   If true, use an in-memory database
    /// </summary>
    public bool UseInMemoryDb { get; set; } = false;

    /// <summary>
    ///   The origins to allow through CORS
    /// </summary>
    public string[] CorsAllowedOrigins { get; init; } = [];
}