using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class HttpRequestModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public HttpRequestGadget.Request GadgetRequest { get; set; } = new HttpRequestGadget.Request { RequestUrl = "http://ipinfo.io/ip" };
        public GadgetResponse<HttpRequestGadget.Result> GadgetResponse { get; set; }

        public HttpRequestModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            var gadget = new HttpRequestGadget(this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}