namespace Bones.Database.Models;

public sealed record DatabaseConfiguration
{
    public string? ConnectionString { get; set; }
    public bool? UseInMemoryDb { get; set; }
}