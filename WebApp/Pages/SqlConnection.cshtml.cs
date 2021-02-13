using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Pages
{
    public class SqlConnectionModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings appSettings;

        [BindProperty]
        public SqlConnectionGadget.Request GadgetRequest { get; set; }
        public GadgetResponse<SqlConnectionGadget.Result> GadgetResponse { get; set; }

        public SqlConnectionModel(ILogger<SqlConnectionModel> logger, IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
            this.GadgetRequest = new SqlConnectionGadget.Request
            {
                CallChainUrls = this.appSettings.DefaultCallChainUrls,
                SqlConnectionString = this.appSettings.DefaultSqlConnectionSqlConnectionString,
                SqlQuery = this.appSettings.DefaultSqlConnectionSqlQuery,
                UseAzureManagedIdentity = this.appSettings.DefaultSqlConnectionUseAzureManagedIdentity,
                AzureManagedIdentityClientId = this.appSettings.DefaultSqlConnectionAzureManagedIdentityClientId
            };
        }

        public async Task OnPost()
        {
            this.logger.LogInformation("Executing SQL Connection page");
            var gadget = new SqlConnectionGadget(this.logger, this.httpClientFactory, Url, this.appSettings);
            this.GadgetResponse = await gadget.ExecuteAsync(this.GadgetRequest);
        }
    }
}