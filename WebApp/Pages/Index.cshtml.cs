using InspectorGadget.WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;

        public InspectorInfo InspectorInfo { get; set; }

        public IndexModel(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.environment = environment;
            this.configuration = configuration;
        }

        public void OnGet()
        {
            this.InspectorInfo = InspectorInfo.Create(this.environment, this.configuration, this.Request, true);
        }
    }
}