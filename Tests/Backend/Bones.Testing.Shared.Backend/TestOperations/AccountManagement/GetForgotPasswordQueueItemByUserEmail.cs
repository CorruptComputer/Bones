using Bones.Database;
using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;

public record GetForgotPasswordQueueItemByUserEmailQuery(string Email) : IRequest<QueryResponse<ForgotPasswordEmailQueue>>;

public class GetForgotPasswordQueueItemByUserEmail(BonesDbContext dbContext) : IRequestHandler<GetForgotPasswordQueueItemByUserEmailQuery, QueryResponse<ForgotPasswordEmailQueue>>
{
    public async Task<QueryResponse<ForgotPasswordEmailQueue>> Handle(GetForgotPasswordQueueItemByUserEmailQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ForgotPasswordEmailQueue
            .FirstOrDefaultAsync(x => x.EmailTo == request.Email, cancellationToken);
    }
}