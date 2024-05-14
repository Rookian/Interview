using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Interview.TaskCleanup;

public class TaskCleanupRequest : IRequest;

public class TaskCleanupCommandHandler(
    Configuration configuration,
    ProductDbContext context,
    ILogger<TaskCreator> logger)
    : IRequestHandler<TaskCleanupRequest>
{
    public async Task Handle(TaskCleanupRequest request, CancellationToken cancellationToken)
    {
        await CompletedTasksCleanup(configuration.CleanupRetentionPeriodInDays, 1000, cancellationToken);
    }

    internal async Task CompletedTasksCleanup(int retentionPeriodInDays, int batchSize, CancellationToken cancellationToken)
    {
        var retentionDate = DateTime.UtcNow.AddDays(-retentionPeriodInDays);
        logger.LogInformation($"Cleaning up the completed Tasks older than {retentionDate}");

        var deletionsInTotal = 0;
        int deletionsPerBatch;

        do
        {
            var tasksToDelete = await context.Tasks
                .Where(x => x.ProcessedAt != null && x.ProcessedAt < retentionDate)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            deletionsPerBatch = tasksToDelete.Count;
            deletionsInTotal += deletionsPerBatch;

            if (deletionsPerBatch <= 0)
                continue;

            context.Tasks.RemoveRange(tasksToDelete);
            await context.SaveChangesAsync(cancellationToken);
        } while (deletionsPerBatch == batchSize && !cancellationToken.IsCancellationRequested);

        logger.LogInformation($"Cleaned up {deletionsInTotal} completed Tasks");
    }
}

public class Configuration
{
    public int CleanupRetentionPeriodInDays { get; set; }
}

public class NavisionExportTask : Entity
{
    public DateTime? ProcessedAt { get; set; }
    public NavisionExportTaskStatus Status { get; set; }
}

public enum NavisionExportTaskStatus
{
    InProgress = 0,
    Succeeded = 1,
    Failed = 2
}

public class TaskCreator
{
    public List<NavisionExportTask> CreateTasks()
    {
        return new List<NavisionExportTask>();
    }
}