using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Pages
{
    public class SqlConnectionModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public SqlConnectionGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<SqlConnectionGadget.Result> GadgetResponse { get; set; }

        public SqlConnectionModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.GadgetRequest = new SqlConnectionGadget.Request
            {
                CallChainUrls = configuration.GetValueOrDefault("DefaultCallChainUrls", null),
                SqlConnectionString = configuration.GetValueOrDefault("DefaultSqlConnectionSqlConnectionString", null),
                SqlQuery = configuration.GetValueOrDefault("DefaultSqlConnectionSqlQuery", "SELECT CONNECTIONPROPERTY('client_net_address')"),
                UseAzureManagedIdentity = configuration.GetValueOrDefault("DefaultSqlConnectionUseAzureManagedIdentity", false)
            };
        }

        public async Task OnPost()
        {
            var gadget = new SqlConnectionGadget(this.httpClientFactory, Url);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}