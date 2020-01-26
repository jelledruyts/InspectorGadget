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
        public async Task<IntrospectorGadget.Response> Introspector([FromBody]IntrospectorGadget.Request request)
        {
            return await IntrospectorGadget.ExecuteAsync(request, Url.Action(nameof(Introspector)), this.Request, this.environment, this.configuration, this.httpClientFactory);
        }

        [HttpPost]
        public async Task<object> DnsLookup([FromBody]DnsLookupGadget.Request request)
        {
            return await DnsLookupGadget.ExecuteAsync(request, Url.Action(nameof(DnsLookup)), this.httpClientFactory);
        }

        [HttpPost]
        public async Task<HttpRequestGadget.Response> HttpRequest([FromBody]HttpRequestGadget.Request request)
        {
            return await HttpRequestGadget.ExecuteAsync(request, Url.Action(nameof(HttpRequest)), this.httpClientFactory);
        }

        [HttpPost]
        public async Task<object> SqlConnection([FromBody]SqlConnectionGadget.Request request)
        {
            return await SqlConnectionGadget.ExecuteAsync(request, Url.Action(nameof(SqlConnection)), this.httpClientFactory);
        }

        [HttpPost]
        public async Task<object> AzureManagedIdentity([FromBody]AzureManagedIdentityGadget.Request request)
        {
            return await AzureManagedIdentityGadget.ExecuteAsync(request, Url.Action(nameof(AzureManagedIdentity)), this.httpClientFactory);
        }
    }
}