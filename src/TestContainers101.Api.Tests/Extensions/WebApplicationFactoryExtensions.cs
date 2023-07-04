using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using TestContainers101.Api.Infra.Persistence;

namespace TestContainers101.Api.Tests.Extensions;

public static class WebApplicationFactoryExtensions
{
    public static async Task EnsureCreatedAsync<T>(this WebApplicationFactory<T> factory, CancellationToken cancellationToken = default) where T : class
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    public static async Task EnsureDeletedAsync<T>(this WebApplicationFactory<T> factory, CancellationToken cancellationToken = default) where T : class
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
    }

    public static async Task EnsureCreatedAndPopulateDataAsync<TProgram, TEntity>(this WebApplicationFactory<TProgram> factory, IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default)
        where TProgram : class
        where TEntity : class
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
