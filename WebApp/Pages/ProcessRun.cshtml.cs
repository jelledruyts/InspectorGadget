using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class ProcessRunModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public ProcessRunGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<ProcessRunGadget.Result> GadgetResponse { get; set; }

        public ProcessRunModel(ILogger<SqlConnectionModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new ProcessRunGadget.Request
            {
                CallChainUrls = this.appSettings.DefaultCallChainUrls,
                FileName = this.appSettings.DefaultProcessRunFileName,
                Arguments = this.appSettings.DefaultProcessRunArguments,
                TimeoutSeconds = this.appSettings.DefaultProcessRunTimeoutSeconds
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing Process Run page");
            var gadget = new ProcessRunGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}