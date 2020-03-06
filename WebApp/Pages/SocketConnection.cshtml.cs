using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class SocketConnectionModel : PageModel
    {
        private const string DefaultHostName = "ipinfo.io";
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public SocketConnectionGadget.Request GadgetRequest { get; set; } = new SocketConnectionGadget.Request
        {
            RequestHostName = DefaultHostName,
            RequestPort = 80,
            RequestBody = "GET / HTTP/1.1\r\nHost: " + DefaultHostName + "\r\nConnection: Close\r\n\r\n",
            ReadResponse = true
        };
        public GadgetResponse<SocketConnectionGadget.Result> GadgetResponse { get; set; }

        public SocketConnectionModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            var gadget = new SocketConnectionGadget(this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}