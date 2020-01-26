using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class DnsLookupModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public DnsLookupGadget.Request GadgetRequest { get; set; } = new DnsLookupGadget.Request();
        public DnsLookupGadget.Response GadgetResponse { get; set; }

        public DnsLookupModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            this.GadgetResponse = await DnsLookupGadget.ExecuteAsync(this.GadgetRequest, Url.Action(nameof(ApiController.DnsLookup), "Api"), this.httpClientFactory);
        }
    }
}