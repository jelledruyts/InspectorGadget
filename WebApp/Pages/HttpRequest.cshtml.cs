using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class HttpRequestModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public HttpRequestGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<HttpRequestGadget.Result> GadgetResponse { get; set; }

        public HttpRequestModel(ILogger<HttpRequestModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.GadgetRequest = new HttpRequestGadget.Request
            {
                CallChainUrls = configuration.GetValueOrDefault("DefaultCallChainUrls", null),
                RequestUrl = configuration.GetValueOrDefault("DefaultHttpRequestUrl", "http://ipinfo.io/ip"),
                RequestHostName = configuration.GetValueOrDefault("DefaultHttpRequestHostName", null)
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing HTTP Request page");
            var gadget = new HttpRequestGadget(this.logger, this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}