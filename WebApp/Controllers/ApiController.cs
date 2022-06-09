using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ApiController : ControllerBase
    {
        public const string ControllerName = "Api";
        private readonly ILogger logger;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        public ApiController(ILogger<ApiController> logger, IWebHostEnvironment environment, IConfiguration configuration, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.environment = environment;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
        }

        [HttpGet]
        [Route("{group?}/{key?}")]
        public object Introspector(string group, string key)
        {
            this.logger.LogInformation("Executing Introspector API on HTTP GET with Group = {Group} and Key = {Key}", group, key);
            var info = InspectorInfo.Create(this.environment, this.configuration, this.Request, false);
            return info.GetPart(group, key);
        }

        [HttpPost]
        public async Task<GadgetResponse<IntrospectorGadget.Result>> Introspector([FromBody] IntrospectorGadget.Request request)
        {
            this.logger.LogInformation("Executing Introspector API");
            var gadget = new IntrospectorGadget(this.logger, this.httpClientFactory, Url, this.Request, this.environment, this.configuration, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<DnsLookupGadget.Result>> DnsLookup([FromBody] DnsLookupGadget.Request request)
        {
            this.logger.LogInformation("Executing DNS Lookup API");
            var gadget = new DnsLookupGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<HttpRequestGadget.Result>> HttpRequest([FromBody] HttpRequestGadget.Request request)
        {
            this.logger.LogInformation("Executing HTTP Request API");
            var gadget = new HttpRequestGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<SqlConnectionGadget.Result>> SqlConnection([FromBody] SqlConnectionGadget.Request request)
        {
            this.logger.LogInformation("Executing SQL Connection API");
            var gadget = new SqlConnectionGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<AzureManagedIdentityGadget.Result>> AzureManagedIdentity([FromBody] AzureManagedIdentityGadget.Request request)
        {
            this.logger.LogInformation("Executing Azure Managed Identity API");
            var gadget = new AzureManagedIdentityGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<SocketConnectionGadget.Result>> SocketConnection([FromBody] SocketConnectionGadget.Request request)
        {
            this.logger.LogInformation("Executing Socket Connection API");
            var gadget = new SocketConnectionGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<ProcessRunGadget.Result>> ProcessRun([FromBody] ProcessRunGadget.Request request)
        {
            this.logger.LogInformation("Executing Process Run");
            var gadget = new ProcessRunGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<SpiffeGadget.Result>> Spiffe([FromBody] SpiffeGadget.Request request)
        {
            this.logger.LogInformation("Executing SPIFFE");
            var gadget = new SpiffeGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<HealthCheckGadget.Result>> HealthCheck([FromBody] HealthCheckGadget.Request request)
        {
            this.logger.LogInformation("Executing Health Check API");
            var gadget = new HealthCheckGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            return await gadget.ExecuteAsync(request);
        }
    }
}