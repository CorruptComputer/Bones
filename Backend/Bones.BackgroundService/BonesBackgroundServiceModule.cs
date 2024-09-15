using Autofac;
using Bones.BackgroundService.Models;
using Bones.Shared.Backend.PipelineBehaviors;
using Bones.Shared.Exceptions;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace Bones.BackgroundService;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesBackgroundServiceModule(IConfiguration config) : Module
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

        BackgroundServiceConfiguration? backgroundTasksConfig = config.GetSection(nameof(BackgroundServiceConfiguration)).Get<BackgroundServiceConfiguration>();
        if (backgroundTasksConfig is null)
        {
            throw new BonesException($"Missing '{nameof(BackgroundServiceConfiguration)}' configuration section.");
        }
        builder.RegisterInstance(backgroundTasksConfig);
    }
}