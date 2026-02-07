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
    ///   The origins to allow through CORS
    /// </summary>
    public string[] CorsAllowedOrigins { get; init; } = [];

    // The remaining properties are for local development or testing, they really shouldn't be used in production

    /// <summary>
    ///   If true, use an in-memory database
    /// </summary>
    public bool UseInMemoryDb { get; set; } = false;

    /// <summary>
    ///   The ID of the in-memory database to use, if any
    /// </summary>
    public Guid? InMemoryDbId { get; set; }

    /// <summary>
    ///   If true, setup the database for testing with default test data
    /// </summary>
    public bool SetupForTesting { get; set; } = false;

    /// <summary>
    ///   If true, setup the database when running the API.
    /// </summary>
    public bool ApiOnly { get; set; } = false;
}