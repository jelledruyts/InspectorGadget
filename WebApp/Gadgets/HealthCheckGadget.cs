using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Gadgets
{
    public class HealthCheckGadget : GadgetBase<HealthCheckGadget.Request, HealthCheckGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public ConfigurableHealthCheckMode HealthCheckMode { get; set; }
            public int FailNextNumberOfTimes { get; set; }
        }

        public class Result
        {
            public ConfigurableHealthCheckMode HealthCheckMode { get; set; }
            public int FailNextNumberOfTimes { get; set; }
            public IList<ConfigurableHealthCheckReport> History { get; set; }
        }

        public HealthCheckGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url, AppSettings appSettings)
            : base(logger, httpClientFactory, url.GetRelativeApiUrl(nameof(ApiController.HealthCheck)), appSettings.DisableHealthCheck)
        {
        }

        protected override Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Executing Health Check configuration for HealthCheckMode {HealthCheckMode} and FailNumberOfTimes {FailNumberOfTimes}", request.HealthCheckMode.ToString(), request.FailNextNumberOfTimes);
            ConfigurableHealthCheck.Configure(request.HealthCheckMode, request.FailNextNumberOfTimes);
            return Task.FromResult(new Result { HealthCheckMode = ConfigurableHealthCheck.Mode, FailNextNumberOfTimes = ConfigurableHealthCheck.FailNextNumberOfTimes, History = ConfigurableHealthCheck.History });
        }
    }
}