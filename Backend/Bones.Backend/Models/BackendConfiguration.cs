namespace Bones.Backend.Models;

public sealed record BackendConfiguration
{
    public string? BackgroundTasksUserEmail { get; set; }

    public string? WebUIBaseUrl { get; set; }
}