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
        private readonly ILogger logger;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        public ApiController(ILogger<ApiController> logger, IWebHostEnvironment environment, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.environment = environment;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
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
        public async Task<GadgetResponse<IntrospectorGadget.Result>> Introspector([FromBody]IntrospectorGadget.Request request)
        {
            this.logger.LogInformation("Executing Introspector API");
            var gadget = new IntrospectorGadget(this.logger, this.httpClientFactory, Url, this.Request, this.environment, this.configuration);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<DnsLookupGadget.Result>> DnsLookup([FromBody]DnsLookupGadget.Request request)
        {
            this.logger.LogInformation("Executing DNS Lookup API");
            var gadget = new DnsLookupGadget(this.logger, this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<HttpRequestGadget.Result>> HttpRequest([FromBody]HttpRequestGadget.Request request)
        {
            this.logger.LogInformation("Executing HTTP Request API");
            var gadget = new HttpRequestGadget(this.logger, this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<SqlConnectionGadget.Result>> SqlConnection([FromBody]SqlConnectionGadget.Request request)
        {
            this.logger.LogInformation("Executing SQL Connection API");
            var gadget = new SqlConnectionGadget(this.logger, this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<AzureManagedIdentityGadget.Result>> AzureManagedIdentity([FromBody]AzureManagedIdentityGadget.Request request)
        {
            this.logger.LogInformation("Executing Azure Managed Identity API");
            var gadget = new AzureManagedIdentityGadget(this.logger, this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<SocketConnectionGadget.Result>> SocketConnection([FromBody]SocketConnectionGadget.Request request)
        {
            this.logger.LogInformation("Executing Socket Connection API");
            var gadget = new SocketConnectionGadget(this.logger, this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }
    }
}