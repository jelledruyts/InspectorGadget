using System;
using System.Data.Common;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Npgsql;

namespace InspectorGadget.WebApp.Gadgets
{
    public class SqlConnectionGadget : GadgetBase<SqlConnectionGadget.Request, SqlConnectionGadget.Result>
    {
        public const string SqlServerDatabaseType = "sqlserver";
        public const string SqlServerDefaultQuery = "SELECT 'User \"' + USER_NAME() + '\" logged in from IP address \"' + CAST(CONNECTIONPROPERTY('client_net_address') AS NVARCHAR) + '\" to database \"' + DB_NAME() + '\" on server \"' + @@SERVERNAME + '\"'";
        public const string PostgreSqlDatabaseType = "postgresql";
        public const string PostgreSqlDefaultQuery = "SELECT CONCAT('User \"', CURRENT_USER, '\" logged in from IP address \"', INET_CLIENT_ADDR(), '\" to database \"', CURRENT_DATABASE(), '\"')";
        public const string MySqlDatabaseType = "mysql";
        public const string MySqlDefaultQuery = "SELECT CONCAT_WS('', 'User \"', USER(), '\" logged in to database \"', DATABASE(), '\"')";

        public class Request : GadgetRequest
        {
            public string DatabaseType { get; set; }
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
            using (var connection = await GetConnectionAsync(request))
            using (var command = connection.CreateCommand())
            {
                await connection.OpenAsync();
                command.CommandText = request.SqlQuery;
                var result = await command.ExecuteScalarAsync();
                return new Result { Output = result?.ToString() };
            }
        }

        private async Task<DbConnection> GetConnectionAsync(Request request)
        {
            if (string.Equals(request.DatabaseType, PostgreSqlDatabaseType, StringComparison.InvariantCultureIgnoreCase))
            {
                return new NpgsqlConnection(request.SqlConnectionString);
            }
            else if (string.Equals(request.DatabaseType, MySqlDatabaseType, StringComparison.InvariantCultureIgnoreCase))
            {
                return new MySqlConnection(request.SqlConnectionString);
            }
            else
            {
                var connection = new SqlConnection(request.SqlConnectionString);
                if (request.UseAzureManagedIdentity)
                {
                    // Request an access token for Azure SQL Database using the current Azure Managed Identity.
                    this.Logger.LogInformation("Acquiring access token using Azure Managed Identity using Client ID \"{ClientId}\"", request.AzureManagedIdentityClientId);
                    // If AzureManagedIdentityClientId is requested, that indicates the User-Assigned Managed Identity to use; if omitted the System-Assigned Managed Identity will be used.
                    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = request.AzureManagedIdentityClientId });
                    var authenticationResult = await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://database.windows.net/.default" }));
                    connection.AccessToken = authenticationResult.Token;
                }
                return connection;
            }
        }
    }
}