using System.Net.Http;
using System.Threading.Tasks;
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
        public GadgetResponse<IntrospectorGadget.Result> GadgetResponse { get; set; }

        public IntrospectorModel(IHttpClientFactory httpClientFactory, IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.environment = environment;
            this.configuration = configuration;
        }

        public async Task OnPost()
        {
            var gadget = new IntrospectorGadget(this.httpClientFactory, Url, this.Request, this.environment, this.configuration);
            this.GadgetResponse =  await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}