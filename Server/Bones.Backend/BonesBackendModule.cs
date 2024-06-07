using Autofac;
using Bones.Backend.PipelineBehaviors;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace Bones.Backend;

/// <summary>
///     Autofac Module for the Bones Backend
/// </summary>
public class BonesBackendModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        MediatRConfigurationBuilder mediatrConfig = MediatRConfigurationBuilder
            .Create(ThisAssembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithCustomPipelineBehaviors([
                typeof(BackendCommandBehavior<>),
                typeof(BackendQueryBehavior<,>)
            ]);

        builder.RegisterMediatR(mediatrConfig.Build());
    }
}