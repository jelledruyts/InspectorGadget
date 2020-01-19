using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace InspectorGadget.WebApp.Pages
{
    public class HttpRequestModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public string RequestUrl { get; set; } = "http://ipinfo.io/ip";
        [BindProperty]
        public string RequestHostName { get; set; }

        public string ResponseBody { get; set; }
        public Exception Exception { get; set; }

        public HttpRequestModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            if (!string.IsNullOrWhiteSpace(this.RequestUrl))
            {
                try
                {
                    var httpClient = this.httpClientFactory.CreateClient();
                    if (!string.IsNullOrWhiteSpace(this.RequestHostName))
                    {
                        httpClient.DefaultRequestHeaders.Add(HeaderNames.Host, this.RequestHostName);
                    }
                    this.ResponseBody = await httpClient.GetStringAsync(this.RequestUrl);
                }
                catch (Exception exc)
                {
                    this.Exception = exc;
                }
            }
        }
    }
}