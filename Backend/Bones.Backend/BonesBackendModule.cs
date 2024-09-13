using Autofac;
using Bones.Backend.Models;
using Bones.Shared.Backend.PipelineBehaviors;
using Bones.Shared.Exceptions;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Backend;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesBackendModule(IConfiguration config, IServiceCollection services) : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        MediatRConfigurationBuilder mediatrConfig = MediatRConfigurationBuilder
            .Create(ThisAssembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithCustomPipelineBehaviors([
                typeof(CommandBehavior<>),
                typeof(QueryBehavior<,>)
            ]);

        builder.RegisterMediatR(mediatrConfig.Build());

        services.AddValidatorsFromAssembly(ThisAssembly, includeInternalTypes: true);

        BackendConfiguration? backgroundTasksConfig = config.GetSection(nameof(BackendConfiguration)).Get<BackendConfiguration>();
        if (backgroundTasksConfig is null)
        {
            throw new BonesException($"Missing '{nameof(BackendConfiguration)}' configuration section.");
        }
        builder.RegisterInstance(backgroundTasksConfig);
    }
}