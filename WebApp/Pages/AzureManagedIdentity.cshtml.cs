using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Pages
{
    public class AzureManagedIdentityModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public AzureManagedIdentityGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<AzureManagedIdentityGadget.Result> GadgetResponse { get; set; }

        public AzureManagedIdentityModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.GadgetRequest = new AzureManagedIdentityGadget.Request
            {
                CallChainUrls = configuration.GetValueOrDefault("DefaultCallChainUrls", null),
                Resource = configuration.GetValueOrDefault("DefaultAzureManagedIdentityResource", "https://management.azure.com/")
            };
        }

        public async Task OnPost()
        {
            var gadget = new AzureManagedIdentityGadget(this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}