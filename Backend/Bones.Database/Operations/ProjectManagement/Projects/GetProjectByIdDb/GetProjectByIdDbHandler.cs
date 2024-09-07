using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Projects.GetProjectByIdDb;

internal sealed class GetProjectByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<GetProjectByIdDbCommand, QueryResponse<Project>>
{
    public async Task<QueryResponse<Project>> Handle(GetProjectByIdDbCommand request, CancellationToken cancellationToken)
    {
        Project? project = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

        if (project is null)
        {
            return QueryResponse<Project>.Fail("Project not found");
        }

        return QueryResponse<Project>.Pass(project);
    }
}