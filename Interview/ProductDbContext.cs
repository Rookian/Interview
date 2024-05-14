using Interview.TaskCleanup;
using Microsoft.EntityFrameworkCore;

namespace Interview;

public class ProductDbContext(DbContextOptions<ProductDbContext> contextOptions) : DbContext(contextOptions)
{
    public DbSet<NavisionExportTask> Tasks { get; set; } = null!;
    public DbSet<Publisher> Publishers { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
}

public static class ProductDbContextExtensions
{
    public static Publisher AddPublisher(this ProductDbContext context, string name)
    {
        var publisher = new Publisher { Name = name };
        context.Add(publisher);
        return publisher;
    }

    public static Item AddItem(this ProductDbContext context, string name)
    {
        var item = new Item { Name = name };
        context.Add(item);
        return item;
    }

    public static NavisionExportTask AddNavisionExportTasks(this ProductDbContext context, NavisionExportTaskStatus status, DateTime processedAt)
    {
        var task = new NavisionExportTask { ProcessedAt = processedAt, Status = status };
        context.Add(task);
        return task;
    }

}

public abstract class Entity
{
    public int Id { get; set; }
}

public class Publisher : Entity
{
    public string Name { get; set; } = null!;
}

public class Item : Entity
{
    public string Name { get; set; } = null!;
}