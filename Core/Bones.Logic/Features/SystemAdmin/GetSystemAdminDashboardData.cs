using Bones.Database.Operations.SystemAdmin;

namespace Bones.Logic.Features.SystemAdmin;

/// <inheritdoc />
public class GetSystemAdminDashboardData(ISender sender) : IRequestHandler<GetSystemAdminDashboardData.Query, QueryResponse<GetSystemAdminDashboardData.Response>>
{
    /// <summary>
    ///   Query to get system admin dashboard data
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<Response>>;

    /// <summary>
    ///   Response containing counts of various entities
    /// </summary>
    /// <param name="UserCount"></param>
    /// <param name="OrganizationCount"></param>
    /// <param name="PtojectCount"></param>
    /// <param name="ItemCount"></param>
    public sealed record Response(int UserCount, int OrganizationCount, int PtojectCount, int ItemCount);

    /// <inheritdoc />
    public async Task<QueryResponse<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        int userCount = await sender.Send(new GetUserCountDb.Query(), cancellationToken);
        int organizationCount = await sender.Send(new GetOrganizationCountDb.Query(), cancellationToken);
        int projectCount = await sender.Send(new GetProjectCountDb.Query(), cancellationToken);
        int itemCount = await sender.Send(new GetItemCountDb.Query(), cancellationToken);

        return QueryResponse<Response>.Pass(new Response(userCount, organizationCount, projectCount, itemCount));
    }
}