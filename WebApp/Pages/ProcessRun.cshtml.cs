using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class ProcessRunModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public ProcessRunGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<ProcessRunGadget.Result> GadgetResponse { get; set; }

        public ProcessRunModel(ILogger<SqlConnectionModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.GadgetRequest = new ProcessRunGadget.Request
            {
                CallChainUrls = configuration.GetValueOrDefault("DefaultCallChainUrls", default(string)),
                FileName = configuration.GetValueOrDefault("DefaultProcessRunFileName", default(string)),
                Arguments = configuration.GetValueOrDefault("DefaultProcessRunArguments", default(string)),
                TimeoutSeconds = configuration.GetValueOrDefault("DefaultProcessRunTimeoutSeconds", default(int?))
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing Process Run page");
            var gadget = new ProcessRunGadget(this.logger, this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}