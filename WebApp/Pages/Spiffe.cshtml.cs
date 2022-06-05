using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MyApp.Namespace
{
    public class SpiffeModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public SPIFFEWorkloadApiGadget.Request GadgetRequest { get; set; }

        public GadgetResponse<SPIFFEWorkloadApiGadget.Result> GadgetResponse { get; set; }

        public SpiffeModel(ILogger<SpiffeModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings){
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new (){
                UnixDomainSocketEndpoint = "/tmp/spire-agent/public/api.sock",
                Audience = "someaudience"
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing SPIFFE workload API query");
            var gadget = new SPIFFEWorkloadApiGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}
