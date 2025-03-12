using System.Reflection;
using Autofac;
using Bones.Shared.Backend.PipelineBehaviors;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace Bones.Testing.Shared.Backend;

internal sealed class UnitTestModule(List<Assembly> additionalMediatRAssemblies) : Module
{
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