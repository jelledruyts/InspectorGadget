using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class HealthCheckModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public HealthCheckGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<HealthCheckGadget.Result> GadgetResponse { get; set; }

        public HealthCheckModel(ILogger<HttpRequestModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new HealthCheckGadget.Request
            {
                CallChainUrls = this.appSettings.DefaultCallChainUrls,
                HealthCheckMode = ConfigurableHealthCheck.Mode,
                FailNextNumberOfTimes = ConfigurableHealthCheck.FailNextNumberOfTimes
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing Health Check page");
            var gadget = new HealthCheckGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}