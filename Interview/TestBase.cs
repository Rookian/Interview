using Interview.TaskCleanup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Interview;

public class TestBase
{
    private readonly ServiceProvider _serviceProvider;

    protected TestBase()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ProductDbContext>(x => x.UseInMemoryDatabase("testdb"));
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<TestBase>());
        services.AddSingleton<Configuration>();
        services.AddLogging();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        return GetService<IMediator>().Send(request, cancellationToken);
    }
    protected Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return GetService<IMediator>().Send(request, cancellationToken);
    }

    protected async Task SetupDataAsync(Action<ProductDbContext> configure)
    {
        var context = GetDbContext();
        configure.Invoke(context);
        await context.SaveChangesAsync();
    }

    protected ProductDbContext GetDbContext()
    {
        return GetService<ProductDbContext>();
    }

    protected T GetService<T>() where T : notnull
    {
        var serviceScope = _serviceProvider.CreateScope();
        return serviceScope.ServiceProvider.GetRequiredService<T>();
    }
}