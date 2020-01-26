using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class SqlConnectionModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public SqlConnectionGadget.Request GadgetRequest { get; set; } = new SqlConnectionGadget.Request { SqlQuery = "SELECT CONNECTIONPROPERTY('client_net_address')" };
        public SqlConnectionGadget.Response GadgetResponse { get; set; }

        public SqlConnectionModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            this.GadgetResponse = await SqlConnectionGadget.ExecuteAsync(this.GadgetRequest, Url.Action(nameof(ApiController.SqlConnection), "Api"), this.httpClientFactory);
        }
    }
}