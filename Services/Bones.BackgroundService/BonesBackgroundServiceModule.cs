using System.Reflection;
using Autofac;
using Bones.Shared.Backend.PipelineBehaviors;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace Bones.BackgroundService;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
/// <param name="additionalMediatRAssemblies">Additional assemblies to scan for MediatR handlers</param>
public class BonesBackgroundServiceModule(List<Assembly> additionalMediatRAssemblies) : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        additionalMediatRAssemblies.Add(ThisAssembly);

        MediatRConfigurationBuilder mediatrConfig = MediatRConfigurationBuilder
            .Create([.. additionalMediatRAssemblies])
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithCustomPipelineBehaviors([
                typeof(CommandBehavior<>),
                typeof(QueryBehavior<,>)
            ]);

        builder.RegisterMediatR(mediatrConfig.Build());
    }
}