using Autofac;
using Bones.Api.Models;
using Bones.Shared.Exceptions;

namespace Bones.Api;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesApiModule(IConfiguration config) : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        ApiConfiguration apiConfig = config.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>()
                                     ?? throw new BonesException($"Missing '{nameof(ApiConfiguration)}' configuration section.");

        builder.RegisterInstance(apiConfig);
    }
}