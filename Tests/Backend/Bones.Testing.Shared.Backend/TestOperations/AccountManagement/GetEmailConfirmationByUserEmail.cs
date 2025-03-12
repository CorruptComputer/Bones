using Bones.Database;
using Bones.Database.DbSets.System;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;

/// <inheritdoc />
public class GetEmailConfirmationByUserEmail(BonesDbContext dbContext) : IRequestHandler<GetEmailConfirmationByUserEmail.Query, QueryResponse<ConfirmationEmailQueue>>
{
    /// <summary>
    ///   TESTING QUERY: Get email confirmation by user email
    /// </summary>
    /// <param name="Email"></param>
    public record Query(string Email) : IRequest<QueryResponse<ConfirmationEmailQueue>>;

    /// <inheritdoc />
    public async Task<QueryResponse<ConfirmationEmailQueue>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ConfirmationEmailQueue
            .FirstOrDefaultAsync(x => x.EmailTo == request.Email, cancellationToken);
    }
}