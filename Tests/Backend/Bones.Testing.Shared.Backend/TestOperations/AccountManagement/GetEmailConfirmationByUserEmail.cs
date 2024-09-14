using Bones.Database;
using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;

public record GetEmailConfirmationByUserEmailQuery(string Email) : IRequest<QueryResponse<ConfirmationEmailQueue>>;

public class GetEmailConfirmationByUserEmail(BonesDbContext dbContext) : IRequestHandler<GetEmailConfirmationByUserEmailQuery, QueryResponse<ConfirmationEmailQueue>>
{
    public async Task<QueryResponse<ConfirmationEmailQueue>> Handle(GetEmailConfirmationByUserEmailQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ConfirmationEmailQueue
            .FirstOrDefaultAsync(x => x.EmailTo == request.Email, cancellationToken);
    }
}