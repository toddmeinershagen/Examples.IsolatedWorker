using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Paycor.FeatureFlags;

namespace Examples.IsolatedWorker
{
    public class CheckFeatureFlag
    {
        private readonly IFlagService _flagService;
        private readonly ILogger _logger;

        public CheckFeatureFlag(ILoggerFactory loggerFactory, IFlagService flagService)
        {
            _flagService = flagService;
            _logger = loggerFactory.CreateLogger<CheckFeatureFlag>();
        }

        [Function(nameof(CheckFeatureFlag))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            string flag
            )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            var isEnabled = await _flagService.IsEnabledAsync(flag, false);
            if (isEnabled)
            {
                await response.WriteStringAsync($"Welcome to Azure Functions!");
            }
            else
            {
                await response.WriteStringAsync("Not enabled.");
            }

            return response;
        }
    }
}
