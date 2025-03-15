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
        configurationBuilder.AddEnvironmentVariables();
        if (environment.IsDevelopment())
        {
            configurationBuilder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        }
        else
        {
            configurationBuilder.AddJsonFile("/etc/bones/backend.json", optional: true, reloadOnChange: true);
        }

        BonesBackendConfiguration? backendConfig = configurationBuilder.GetSection("BonesBackendConfiguration").Get<BonesBackendConfiguration>();

        if (backendConfig is null)
        {
            if (environment.IsDevelopment())
            {
                throw new BonesException("BonesBackendConfiguration is missing from appsettings.Development.json");
            }
            else
            {
                throw new BonesException("BonesBackendConfiguration is missing from /etc/bones/backend.json");
            }
        }

        return backendConfig;
    }
}
