namespace Bones.AppHost;

/// <summary>
///     Should be self-explanatory as to what this is.
/// </summary>
public static class Program
{
    /// <summary>
    ///     The main character of the project.
    /// </summary>
    /// <param name="args">Arg, I'm a pirate.</param>
    public static async Task Main(string[] args)
    {
        await DistributedApplication.CreateBuilder(args).BuildBonesAppHost().RunBonesAppHostAsync();
    }

    private static DistributedApplication BuildBonesAppHost(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<ProjectResource> backgroundService = builder.AddProject<Projects.Bones_BackgroundService>(ServiceNames.BackgroundService);

        IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Bones_Api>(ServiceNames.Api)
                                                       .WaitFor(backgroundService);
        
        builder.AddProject<Projects.Bones_WebUI>(ServiceNames.WebUI)
               .WithExternalHttpEndpoints()
               .WithReference(api)
               .WaitFor(api);
        
        return builder.Build();
    }

    private static async Task RunBonesAppHostAsync(this DistributedApplication app)
    {
        await app.RunAsync();
    }
}