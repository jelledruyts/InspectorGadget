using System;
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

        public HttpRequestGadget.Response GadgetResponse { get; set; }
        public Exception Exception { get; set; }

        public HttpRequestModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            try
            {
                this.GadgetResponse = await HttpRequestGadget.ExecuteAsync(this.GadgetRequest, this.httpClientFactory);
            }
            catch (Exception exc)
            {
                this.Exception = exc;
            }
        }
    }
}