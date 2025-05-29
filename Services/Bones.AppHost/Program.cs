using System.Reflection;

namespace Bones.AppHost;

/// <summary>
///   This app host is only used for local development and testing purposes, its not intended for production use.
/// </summary>
public static class Program
{
    /// <summary>
    ///   Entry point
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        await DistributedApplication.CreateBuilder(args).BuildBonesAppHost().RunBonesAppHostAsync();
    }

    private static DistributedApplication BuildBonesAppHost(this IDistributedApplicationBuilder builder)
    {
        Assembly myAss = Assembly.GetExecutingAssembly();

        if (myAss.Location.Contains("Bones.Api.IntegrationTests"))
        {
            // Skip initializing the rest, no reason to slow down the tests for them
            builder.AddProject<Projects.Bones_Api>(ServiceNames.Api);
        }
        else
        {
            IResourceBuilder<ProjectResource> backgroundService = builder.AddProject<Projects.Bones_BackgroundService>(ServiceNames.BackgroundService);

            IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Bones_Api>(ServiceNames.Api)
                                                           .WaitFor(backgroundService);

            builder.AddProject<Projects.Bones_WebUI>(ServiceNames.WebUI)
                   .WithExternalHttpEndpoints()
                   .WithReference(api)
                   .WaitFor(api);
        }

        return builder.Build();
    }

    private static async Task RunBonesAppHostAsync(this DistributedApplication app)
    {
        await app.RunAsync();
    }
}