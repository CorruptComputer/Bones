using System.Reflection;
using Autofac;
using Bones.BackgroundService.Models;
using Bones.Shared.Backend.PipelineBehaviors;
using Bones.Shared.Exceptions;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace Bones.BackgroundService;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesBackgroundServiceModule(IConfiguration config, List<Assembly> additionalMediatRAssemblies) : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        additionalMediatRAssemblies.Add(ThisAssembly);

        MediatRConfigurationBuilder mediatrConfig = MediatRConfigurationBuilder
            .Create(additionalMediatRAssemblies.ToArray())
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithCustomPipelineBehaviors([
                typeof(CommandBehavior<>),
                typeof(QueryBehavior<,>)
            ]);

        builder.RegisterMediatR(mediatrConfig.Build());

        BackgroundServiceConfiguration? backgroundTasksConfig = config.GetSection(nameof(BackgroundServiceConfiguration)).Get<BackgroundServiceConfiguration>();
        if (backgroundTasksConfig is null)
        {
            throw new BonesException($"Missing '{nameof(BackgroundServiceConfiguration)}' configuration section.");
        }
        builder.RegisterInstance(backgroundTasksConfig);
    }
}