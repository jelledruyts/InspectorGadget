using InspectorGadget.WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;

        public InspectorInfo InspectorInfo { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.logger = logger;
            this.environment = environment;
            this.configuration = configuration;
        }

        public void OnGet()
        {
            this.logger.LogInformation("Executing Index page");
            this.InspectorInfo = InspectorInfo.Create(this.environment, this.configuration, this.Request, true);
        }
    }
}