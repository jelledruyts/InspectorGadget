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
    public class DnsLookupModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public DnsLookupGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<DnsLookupGadget.Result> GadgetResponse { get; set; }

        public DnsLookupModel(ILogger<DnsLookupModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.GadgetRequest = new DnsLookupGadget.Request
            {
                CallChainUrls = configuration.GetValueOrDefault("DefaultCallChainUrls", null),
                Host = configuration.GetValueOrDefault("DefaultDnsLookupHost", null)
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing DNS Lookup page");
            var gadget = new DnsLookupGadget(this.logger, this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}