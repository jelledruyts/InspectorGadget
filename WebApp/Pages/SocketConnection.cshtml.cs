using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class SocketConnectionModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public SocketConnectionGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<SocketConnectionGadget.Result> GadgetResponse { get; set; }

        public SocketConnectionModel(ILogger<SocketConnectionModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new SocketConnectionGadget.Request
            {
                CallChainUrls = this.appSettings.DefaultCallChainUrls,
                RequestHostName = this.appSettings.DefaultSocketConnectionRequestHostName,
                RequestPort = this.appSettings.DefaultSocketConnectionRequestPort,
                RequestBody = this.appSettings.DefaultSocketConnectionRequestBody,
                ReadResponse = this.appSettings.DefaultSocketConnectionReadResponse
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing Socket Connection page");
            var gadget = new SocketConnectionGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}