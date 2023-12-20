using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace TestContainers101.Function
{
    public class HealthChecksHttpTrigger
    {
        private readonly ILogger _logger;

        public HealthChecksHttpTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HealthChecksHttpTrigger>();
        }


        [Function("HealthChecksHttpTrigger")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel., "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
