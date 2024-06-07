using Autofac;
using Bones.Database.Infrastructure.MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace Bones.Database;

/// <summary>
///   Autofac module for the Bones database
/// </summary>
public class BonesDatabaseModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        MediatRConfigurationBuilder mediatrConfig = MediatRConfigurationBuilder
            .Create(ThisAssembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithCustomPipelineBehaviors([
                typeof(DbCommandPipelineBehavior<>),
                typeof(DbQueryPipelineBehavior<,>)
            ]);

        builder.RegisterMediatR(mediatrConfig.Build());
    }
}