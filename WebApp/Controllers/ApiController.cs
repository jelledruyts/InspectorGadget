using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ApiController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        public ApiController(IWebHostEnvironment environment, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.environment = environment;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("{group?}/{key?}")]
        public object Introspector(string group, string key)
        {
            var info = InspectorInfo.Create(this.environment, this.configuration, this.Request, false);
            return info.GetPart(group, key);
        }

        [HttpPost]
        public async Task<GadgetResponse<IntrospectorGadget.Result>> Introspector([FromBody]IntrospectorGadget.Request request)
        {
            var gadget = new IntrospectorGadget(this.httpClientFactory, Url, this.Request, this.environment, this.configuration);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<DnsLookupGadget.Result>> DnsLookup([FromBody]DnsLookupGadget.Request request)
        {
            var gadget = new DnsLookupGadget(this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<HttpRequestGadget.Result>> HttpRequest([FromBody]HttpRequestGadget.Request request)
        {
            var gadget = new HttpRequestGadget(this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<SqlConnectionGadget.Result>> SqlConnection([FromBody]SqlConnectionGadget.Request request)
        {
            var gadget = new SqlConnectionGadget(this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }

        [HttpPost]
        public async Task<GadgetResponse<AzureManagedIdentityGadget.Result>> AzureManagedIdentity([FromBody]AzureManagedIdentityGadget.Request request)
        {
            var gadget = new AzureManagedIdentityGadget(this.httpClientFactory, Url);
            return await gadget.ExecuteAsync(request);
        }
    }
}