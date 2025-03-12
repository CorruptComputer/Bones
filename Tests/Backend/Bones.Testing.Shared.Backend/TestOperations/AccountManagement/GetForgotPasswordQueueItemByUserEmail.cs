using Bones.Database;
using Bones.Database.DbSets.System;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;

/// <inheritdoc />
public class GetForgotPasswordQueueItemByUserEmail(BonesDbContext dbContext) : IRequestHandler<GetForgotPasswordQueueItemByUserEmail.Query, QueryResponse<ForgotPasswordEmailQueue>>
{
    /// <summary>
    ///   TESTING QUERY: Get forgot password queue item by user email
    /// </summary>
    /// <param name="Email"></param>
    public record Query(string Email) : IRequest<QueryResponse<ForgotPasswordEmailQueue>>;

    /// <inheritdoc />
    public async Task<QueryResponse<ForgotPasswordEmailQueue>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ForgotPasswordEmailQueue
            .FirstOrDefaultAsync(x => x.EmailTo == request.Email, cancellationToken);
    }
}