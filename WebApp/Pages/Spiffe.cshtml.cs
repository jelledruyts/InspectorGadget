using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class SpiffeModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public SpiffeGadget.Request GadgetRequest { get; set; }

        public GadgetResponse<SpiffeGadget.Result> GadgetResponse { get; set; }

        public SpiffeModel(ILogger<SpiffeModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new()
            {
                UnixDomainSocketEndpoint = this.appSettings.DefaultSpiffeUnixDomainSocketEndpoint,
                Audience = this.appSettings.DefaultSpiffeAudience
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing SPIFFE page");
            var gadget = new SpiffeGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}