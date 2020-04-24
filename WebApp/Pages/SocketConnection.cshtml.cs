using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Pages
{
    public class SocketConnectionModel : PageModel
    {
        private const string DefaultHostName = "ipinfo.io";
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public SocketConnectionGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<SocketConnectionGadget.Result> GadgetResponse { get; set; }

        public SocketConnectionModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.GadgetRequest = new SocketConnectionGadget.Request
            {
                CallChainUrls = configuration.GetValueOrDefault("DefaultCallChainUrls", null),
                RequestHostName = configuration.GetValueOrDefault("DefaultSocketConnectionRequestHostName", DefaultHostName),
                RequestPort = configuration.GetValueOrDefault("DefaultSocketConnectionRequestPort", 80),
                RequestBody = configuration.GetValueOrDefault("DefaultSocketConnectionRequestBody", "GET / HTTP/1.1\r\nHost: " + DefaultHostName + "\r\nConnection: Close\r\n\r\n"),
                ReadResponse = configuration.GetValueOrDefault("DefaultSocketConnectionReadResponse", true)
            };
        }

        public async Task OnPost()
        {
            var gadget = new SocketConnectionGadget(this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}