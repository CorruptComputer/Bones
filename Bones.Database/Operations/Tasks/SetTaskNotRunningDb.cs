namespace Bones.Database.Operations.Tasks;

//public class SetTaskNotRunningDbCommand(string TaskName) : IRequest<DbCommandResponse>;
//
//internal class SetTaskNotRunningDbHandler(BonesDbContext dbContext) : IRequestHandler<SetTaskNotRunningDbCommand, DbCommandResponse>
//{
//    public async Task<DbCommandResponse> Handle(SetTaskNotRunningDbCommand request, CancellationToken cancellationToken)
//    {
//        
//        TaskSchedule? schedule = await dbContext.TaskSchedules.FirstOrDefaultAsync(ts => ts.TaskName == request.TaskName, cancellationToken);
//        
//        return new()
//        {
//            Success = true,
//            Result = new(schedule?.LastRunTime, schedule?.NextRunTime, schedule?.Running ?? false)
//        };
//    }
//}