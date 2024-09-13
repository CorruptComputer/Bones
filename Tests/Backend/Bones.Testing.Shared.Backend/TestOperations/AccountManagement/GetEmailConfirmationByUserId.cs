using Bones.Database;
using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;

public record GetEmailConfirmationByUserIdQuery(Guid UserId) : IRequest<QueryResponse<ConfirmationEmailQueue>>;

public class GetEmailConfirmationByUserId(BonesDbContext dbContext) : IRequestHandler<GetEmailConfirmationByUserIdQuery, QueryResponse<ConfirmationEmailQueue>>
{
    public async Task<QueryResponse<ConfirmationEmailQueue>> Handle(GetEmailConfirmationByUserIdQuery request, CancellationToken cancellationToken)
    {
        List<ConfirmationEmailQueue> confirmationEmailQueues = await dbContext.ConfirmationEmailQueue.ToListAsync(cancellationToken);
        return await dbContext.ConfirmationEmailQueue
            .Include(confirmationEmailQueue => confirmationEmailQueue.User)
            .FirstOrDefaultAsync(x => x.User.Id == request.UserId, cancellationToken);
    }
}