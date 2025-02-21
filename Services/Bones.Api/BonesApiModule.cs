using System.Reflection;
using Autofac;
using Bones.Shared.Backend.PipelineBehaviors;
using Bones.Shared.Exceptions;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace Bones.Api;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesApiModule(IConfiguration config, List<Assembly> additionalMediatRAssemblies) : Module
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

        ApiConfiguration apiConfig = config.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>()
                                     ?? throw new BonesException($"Missing '{nameof(ApiConfiguration)}' configuration section.");

        builder.RegisterInstance(apiConfig);
    }
}