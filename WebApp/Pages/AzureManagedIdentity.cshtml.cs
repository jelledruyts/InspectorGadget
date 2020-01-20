using System;
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

        public AzureManagedIdentityGadget.Response GadgetResponse { get; set; }
        public Exception Exception { get; set; }

        public AzureManagedIdentityModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            try
            {
                this.GadgetResponse = await AzureManagedIdentityGadget.ExecuteAsync(this.GadgetRequest, this.httpClientFactory);
            }
            catch (Exception exc)
            {
                this.Exception = exc;
            }
        }
    }
}