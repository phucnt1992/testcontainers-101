namespace TestContainers101.Api.Tests.Extensions;

using DotNet.Testcontainers.Containers;

using Polly;

public static class TestContainersExtensions
{
    public static Task StartWithWaitAndRetryAsync(this IContainer container, int retryCount = 3, int retryDelay = 3)
        => Policy
            .Handle<AggregateException>()
            .Or<InvalidOperationException>()
            .WaitAndRetryAsync(retryCount, _ => TimeSpan.FromSeconds(retryCount * retryDelay))
            .ExecuteAsync(async () => await container.StartAsync());
}
