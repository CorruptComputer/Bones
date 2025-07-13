using System.Reflection;
using Autofac;
using Bones.Shared.Backend.PipelineBehaviors;
using Questy.Autofac;
using Questy.Autofac.Builder;
using Module = Autofac.Module;

namespace Bones.BackgroundService;

/// <summary>
///   Autofac module for the Bones database
/// </summary>
/// <param name="additionalQuestyAssemblies">Additional assemblies to scan for Questy handlers</param>
public class BonesBackgroundServiceModule(List<Assembly> additionalQuestyAssemblies) : Module
{
    /// <inheritdoc />
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