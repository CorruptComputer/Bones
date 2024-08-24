namespace Bones.Database.Models;

public sealed record DatabaseConfiguration
{
    public required string ConnectionString { get; init; }
    public required bool UseInMemoryDb { get; init; }
}