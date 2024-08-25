namespace Bones.Api.Models;

public sealed record ApiConfiguration
{
    public string? WebUIBaseUrl { get; set; }
    
    public string? ApiBaseUrl { get; set; }
}