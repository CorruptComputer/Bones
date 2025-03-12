using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Database;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesDatabaseModule(IServiceCollection services) : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        services.AddValidatorsFromAssembly(ThisAssembly, includeInternalTypes: true);
    }
}