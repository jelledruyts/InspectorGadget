using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Gadgets
{
    public class SqlConnectionGadget : GadgetBase<SqlConnectionGadget.Request, SqlConnectionGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string SqlConnectionString { get; set; }
            public string SqlQuery { get; set; }
            public bool UseAzureManagedIdentity { get; set; }
            public string AzureManagedIdentityClientId { get; set; }
        }

        public class Result
        {
            public string Output { get; set; }
        }

        public SqlConnectionGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url, AppSettings appSettings)
            : base(logger, httpClientFactory, url.GetRelativeApiUrl(nameof(ApiController.SqlConnection)), appSettings.DisableSqlConnection)
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Executing SQL Connection with SqlQuery {SqlQuery}", request.SqlQuery);
            using (var connection = new SqlConnection(request.SqlConnectionString))
            using (var command = connection.CreateCommand())
            {
                if (request.UseAzureManagedIdentity)
                {
                    // Request an access token for Azure SQL Database using the current Azure Managed Identity.
                    this.Logger.LogInformation("Acquiring access token using Azure Managed Identity using Client ID \"{ClientId}\"", request.AzureManagedIdentityClientId);
                    // If AzureManagedIdentityClientId is requested, that indicates the User-Assigned Managed Identity to use; if omitted the System-Assigned Managed Identity will be used.
                    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = request.AzureManagedIdentityClientId });
                    var authenticationResult = await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://database.windows.net/.default" }));
                    connection.AccessToken = authenticationResult.Token;
                }
                connection.Open();
                command.CommandText = request.SqlQuery;
                var result = await command.ExecuteScalarAsync();
                return new Result { Output = result?.ToString() };
            }
        }
    }
}