using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Interview.TaskCleanup.NavisionExportTaskStatus;

namespace Interview.TaskCleanup;

public class TaskCleanupTests : TestBase
{
    [Fact]
    public async Task Should_cleanup_successful_processed_tasks_older_than_90_days()
    {
        // Arrange
        GetService<Configuration>().CleanupRetentionPeriodInDays = 90;

        await SetupDataAsync(x =>
        {
            x.AddNavisionExportTasks(Succeeded, DateTime.Now.AddDays(-80));

            x.AddNavisionExportTasks(Succeeded, DateTime.Now.AddDays(-91));
            x.AddNavisionExportTasks(Succeeded, DateTime.Now.AddDays(-92));
            x.AddNavisionExportTasks(Succeeded, DateTime.Now.AddDays(-93));
        });

        // Act
        await Send(new TaskCleanupRequest());

        // Assert
        var db = GetDbContext();
        var count = await db.Tasks.CountAsync();
        count.Should().Be(1);
    }
}