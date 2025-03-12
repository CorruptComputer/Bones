using System.Reflection;
using Autofac;
using Bones.Shared.Backend.PipelineBehaviors;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace Bones.Api;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
/// <param name="additionalMediatRAssemblies">Additional assemblies to register with MediatR</param>
public class BonesApiModule(List<Assembly> additionalMediatRAssemblies) : Module
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
    }
}