using Autofac;
using Bones.Api.Models;
using Bones.Shared.Backend.PipelineBehaviors;
using Bones.Shared.Exceptions;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace Bones.Api;

/// <summary>
///     Autofac module for the Bones database
/// </summary>
public class BonesApiModule(IConfiguration config) : Module
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

        ApiConfiguration? apiConfig = config.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>();
        if (apiConfig is null)
        {
            throw new BonesException($"Missing '{nameof(ApiConfiguration)}' configuration section.");
        }
        builder.RegisterInstance(apiConfig);
    }
}