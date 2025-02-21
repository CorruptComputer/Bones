using Autofac;
using Bones.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Logic;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesBackendModule(IServiceCollection services) : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        services.AddValidatorsFromAssembly(ThisAssembly, includeInternalTypes: true);
    }
}