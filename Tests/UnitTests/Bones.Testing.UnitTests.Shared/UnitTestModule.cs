using System.Reflection;
using Autofac;
using Bones.Shared.Backend.PipelineBehaviors;
using Questy.Autofac;
using Questy.Autofac.Builder;
using Module = Autofac.Module;

namespace Bones.Testing.UnitTests.Shared;

internal sealed class UnitTestModule(List<Assembly> additionalQuestyAssemblies) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        additionalQuestyAssemblies.Add(ThisAssembly);

        QuestyConfigurationBuilder questyConfig = QuestyConfigurationBuilder
            .Create([.. additionalQuestyAssemblies])
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithCustomPipelineBehaviors([
                typeof(CommandBehavior<>),
                typeof(QueryBehavior<,>)
            ]);

        builder.RegisterQuesty(questyConfig.Build());
    }
}