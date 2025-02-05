using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives;

namespace Bones.Logic.Features.Projects.Initiatives;

/// <summary>
///   Gets the initiative by ID
/// </summary>
/// <param name="InitiativeId"></param>
/// <param name="RequestingUser"></param>
public sealed record GetInitiativeByIdQuery(Guid InitiativeId, BonesUser RequestingUser) : IRequest<QueryResponse<Initiative>>;

internal sealed class GetInitiativeByIdQueryValidator : AbstractValidator<GetInitiativeByIdQuery>
{
    public GetInitiativeByIdQueryValidator()
    {
        RuleFor(x => x.InitiativeId).NotNull().NotEmpty();
        RuleFor(x => x.RequestingUser).NotNull();
    }
}

internal sealed class GetInitiativeByIdHandler(ISender sender) : IRequestHandler<GetInitiativeByIdQuery, QueryResponse<Initiative>>
{
    public async Task<QueryResponse<Initiative>> Handle(GetInitiativeByIdQuery request, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetInitiativesByIdDbQuery(request.InitiativeId), cancellationToken);
    }
}