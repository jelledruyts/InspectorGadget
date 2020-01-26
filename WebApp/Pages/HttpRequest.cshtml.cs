using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
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
        public HttpRequestGadget.Response GadgetResponse { get; set; }

        public HttpRequestModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            this.GadgetResponse = await HttpRequestGadget.ExecuteAsync(this.GadgetRequest, Url.Action(nameof(ApiController.HttpRequest), "Api"), this.httpClientFactory);
        }
    }
}