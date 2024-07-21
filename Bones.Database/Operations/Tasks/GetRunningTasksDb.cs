namespace Bones.Database.Operations.Tasks;

//public record GetRunningTasksDbQuery : IRequest<DbQueryResponse<List<string>>>;
//
//internal class GetRunningTasksDbHandler(BonesDbContext dbContext) : IRequestHandler<GetRunningTasksDbQuery, DbQueryResponse<List<string>>>
//{
//    public async Task<DbQueryResponse<List<string>>> Handle(GetRunningTasksDbQuery request, CancellationToken cancellationToken)
//    {
//        TaskSchedule? schedule = await dbContext.TaskSchedules.FirstOrDefaultAsync(ts => ts.TaskName == request.TaskName, cancellationToken);
//        
//        return new()
//        {
//            Success = true,
//            Result = new(schedule?.LastRunTime, schedule?.NextRunTime, schedule?.Running ?? false)
//        };
//    }
//}