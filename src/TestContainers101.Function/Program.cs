using Microsoft.Extensions.Hosting;

namespace TestContainers101.Function;

public static class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .Build();

        host.Run();
    }
}

