using Autofac;
using Bones.Database.Models;
using Bones.Shared.Backend.PipelineBehaviors;
using Bones.Shared.Exceptions;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Microsoft.Extensions.Configuration;

namespace Bones.Database;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesDatabaseModule(IConfiguration config) : Module
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

        DatabaseConfiguration? backgroundTasksConfig = config.GetSection(nameof(DatabaseConfiguration)).Get<DatabaseConfiguration>();
        if (backgroundTasksConfig is null)
        {
            throw new BonesException($"Missing '{nameof(DatabaseConfiguration)}' configuration section.");
        }
        builder.RegisterInstance(backgroundTasksConfig);
    }
}