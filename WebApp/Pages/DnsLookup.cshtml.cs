using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class DnsLookupModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public DnsLookupGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<DnsLookupGadget.Result> GadgetResponse { get; set; }

        public DnsLookupModel(ILogger<DnsLookupModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new DnsLookupGadget.Request
            {
                CallChainUrls = this.appSettings.DefaultCallChainUrls,
                Host = this.appSettings.DefaultDnsLookupHost
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing DNS Lookup page");
            var gadget = new DnsLookupGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}