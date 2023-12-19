using Microsoft.Extensions.DependencyInjection;

using TestContainers101.Api.Infra.Persistence;
using TestContainers101.Api.Tests.Fixtures;

namespace TestContainers101.Api.Tests.Extensions;

public static class TestWebApplicationFactoryExtensions
{
    public static async Task EnsureCreatedAsync<T>(this TestWebApplicationFactory<T> factory, CancellationToken cancellationToken = default) where T : class
    {
        await using var scope = factory.Instance.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    public static async Task EnsureCreatedAndPopulateDataAsync<TProgram, TEntity>(this TestWebApplicationFactory<TProgram> factory, IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default)
        where TProgram : class
        where TEntity : class
    {
        await using var scope = factory.Instance.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
