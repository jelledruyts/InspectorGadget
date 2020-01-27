using System.Net.Http;
using System.Threading.Tasks;
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
        public GadgetResponse<SqlConnectionGadget.Result> GadgetResponse { get; set; }

        public SqlConnectionModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            var gadget = new SqlConnectionGadget(this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}