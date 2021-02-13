using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class AzureManagedIdentityModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public AzureManagedIdentityGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<AzureManagedIdentityGadget.Result> GadgetResponse { get; set; }

        public AzureManagedIdentityModel(ILogger<AzureManagedIdentityModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new AzureManagedIdentityGadget.Request
            {
                CallChainUrls = this.appSettings.DefaultCallChainUrls,
                Scopes = this.appSettings.DefaultAzureManagedIdentityScopes,
                AzureManagedIdentityClientId = this.appSettings.DefaultAzureManagedIdentityClientId
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing Azure Managed Identity page");
            var gadget = new AzureManagedIdentityGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}