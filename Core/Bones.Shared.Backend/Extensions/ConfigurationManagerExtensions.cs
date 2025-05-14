using Bones.Shared.Backend.Models;
using Bones.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Bones.Shared.Backend.Extensions;

/// <summary>
///   Extensions for the ConfigurationManager class
/// </summary>
public static class ConfigurationManagerExtensions
{
    /// <summary>
    ///   Adds the BonesBackendConfiguration to the configuration builder
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static BonesBackendConfiguration AddBonesBackendConfiguration(this ConfigurationManager configurationBuilder, IHostEnvironment environment)
    {
        string configFilePath = environment.IsDevelopment()
                ? "appsettings.Development.json"
                : "/etc/bones/backend.json";

        Console.WriteLine($"Loading configuration from: {configFilePath}");

        configurationBuilder.AddJsonFile(configFilePath, optional: true, reloadOnChange: true);
        configurationBuilder.AddEnvironmentVariables();

        BonesBackendConfiguration? backendConfig = configurationBuilder.GetSection("BonesBackendConfiguration").Get<BonesBackendConfiguration>();

        if (backendConfig is null)
        {
            Console.WriteLine($"BonesBackendConfiguration is missing from {configFilePath}");
            throw new BonesException($"BonesBackendConfiguration is missing from {configFilePath}");
        }

        return backendConfig;
    }
}
