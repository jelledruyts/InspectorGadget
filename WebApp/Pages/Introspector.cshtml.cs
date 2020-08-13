using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class IntrospectorModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        
        [BindProperty]
        public IntrospectorGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<IntrospectorGadget.Result> GadgetResponse { get; set; }

        public IntrospectorModel(ILogger<IntrospectorModel> logger, IHttpClientFactory httpClientFactory, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.environment = environment;
            this.configuration = configuration;
            this.GadgetRequest = new IntrospectorGadget.Request
            {
                CallChainUrls = configuration.GetValueOrDefault("DefaultCallChainUrls", null),
                Group = configuration.GetValueOrDefault("DefaultIntrospectorGroup", null),
                Key = configuration.GetValueOrDefault("DefaultIntrospectorKey", null)
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing Introspector page");
            var gadget = new IntrospectorGadget(this.logger, this.httpClientFactory, Url, this.Request, this.environment, this.configuration);
            this.GadgetResponse =  await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}