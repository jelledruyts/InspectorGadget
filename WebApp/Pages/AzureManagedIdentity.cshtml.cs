using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class AzureManagedIdentityModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public AzureManagedIdentityGadget.Request GadgetRequest { get; set; } = new AzureManagedIdentityGadget.Request { Resource = "https://management.azure.com/" };
        public GadgetResponse<AzureManagedIdentityGadget.Result> GadgetResponse { get; set; }

        public AzureManagedIdentityModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            var gadget = new AzureManagedIdentityGadget(this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}