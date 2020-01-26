using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Pages
{
    public class IntrospectorModel : PageModel
    {
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        
        [BindProperty]
        public IntrospectorGadget.Request GadgetRequest { get; set; } = new IntrospectorGadget.Request();
        public IntrospectorGadget.Response GadgetResponse { get; set; }

        public IntrospectorModel(IWebHostEnvironment environment, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.environment = environment;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            this.GadgetResponse =  await IntrospectorGadget.ExecuteAsync(this.GadgetRequest, Url.Action(nameof(ApiController.Introspector), "Api"), this.Request, this.environment, this.configuration, this.httpClientFactory);
        }
    }
}