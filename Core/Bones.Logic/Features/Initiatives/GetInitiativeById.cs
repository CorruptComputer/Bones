using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives;

namespace Bones.Logic.Features.Initiatives;

/// <inheritdoc />
public sealed class GetInitiativeById(ISender sender) : IRequestHandler<GetInitiativeById.Query, QueryResponse<Initiative>>
{
    /// <summary>
    ///   Gets the initiative by ID
    /// </summary>
    /// <param name="InitiativeId"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(Guid InitiativeId, BonesUser RequestingUser) : IRequest<QueryResponse<Initiative>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<Initiative>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetInitiativesByIdDb.Query(request.InitiativeId), cancellationToken);
    }
}