using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class HttpRequestModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public HttpRequestGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<HttpRequestGadget.Result> GadgetResponse { get; set; }

        public HttpRequestModel(ILogger<HttpRequestModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new HttpRequestGadget.Request
            {
                CallChainUrls = this.appSettings.DefaultCallChainUrls,
                RequestUrl = this.appSettings.DefaultHttpRequestUrl,
                RequestHostName = this.appSettings.DefaultHttpRequestHostName,
                IgnoreServerCertificateErrors = this.appSettings.DefaultHttpRequestIgnoreServerCertificateErrors
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing HTTP Request page");
            var gadget = new HttpRequestGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}