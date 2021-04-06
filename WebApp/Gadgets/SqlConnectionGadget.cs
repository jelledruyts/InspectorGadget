using System;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Npgsql;

namespace InspectorGadget.WebApp.Gadgets
{
    public class SqlConnectionGadget : GadgetBase<SqlConnectionGadget.Request, SqlConnectionGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public SqlConnectionDatabaseType DatabaseType { get; set; }
            public string SqlConnectionString { get; set; }
            public string SqlConnectionStringSuffix { get; set; }
            public string SqlQuery { get; set; }
            public bool UseAzureManagedIdentity { get; set; }
            public string AzureManagedIdentityClientId { get; set; }

            public string SqlConnectionStringValue => this.SqlConnectionString + this.SqlConnectionStringSuffix;
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
            this.Logger.LogInformation("Executing SQL Connection for DatabaseType {DatabaseType} with SqlQuery {SqlQuery}", request.DatabaseType.ToString(), request.SqlQuery);

            if (request.DatabaseType == SqlConnectionDatabaseType.CosmosDB)
            {
                using (var iterator = GetFeedIterator<object>(request))
                {
                    var resultSet = await iterator.ReadNextAsync();
                    return new Result { Output = resultSet.FirstOrDefault()?.ToString() };
                }
            }
            else
            {
                using (var connection = await GetDbConnectionAsync(request))
                using (var command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    command.CommandText = request.SqlQuery;
                    var result = await command.ExecuteScalarAsync();
                    return new Result { Output = result?.ToString() };
                }
            }
        }

        private async Task<DbConnection> GetDbConnectionAsync(Request request)
        {
            if (request.DatabaseType == SqlConnectionDatabaseType.SqlServer)
            {
                var connection = new SqlConnection(request.SqlConnectionStringValue);
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
            else if (request.DatabaseType == SqlConnectionDatabaseType.PostgreSql)
            {
                return new NpgsqlConnection(request.SqlConnectionStringValue);
            }
            else if (request.DatabaseType == SqlConnectionDatabaseType.MySql)
            {
                return new MySqlConnection(request.SqlConnectionStringValue);
            }
            else
            {
                throw new NotSupportedException($"\"{request.DatabaseType.ToString()}\" is not a supported ADO.NET database type");
            }
        }

        private static FeedIterator<T> GetFeedIterator<T>(Request request)
        {
            var client = new CosmosClient(request.SqlConnectionStringValue);
            // See if a Database and Container were specified in the connection string.
            var connectionString = new DbConnectionStringBuilder { ConnectionString = request.SqlConnectionStringValue };
            if (!connectionString.TryGetValue("Database", out object databaseName))
            {
                // No Database specified, return a query iterator for the Account.
                return client.GetDatabaseQueryIterator<T>(request.SqlQuery);
            }
            if (!connectionString.TryGetValue("Container", out object containerName))
            {
                // No Container specified, return a query iterator for the Database.
                return client.GetDatabase((string)databaseName).GetContainerQueryIterator<T>(request.SqlQuery);
            }
            // A Database and Container were specified, return a query iterator for the Container.
            return client.GetDatabase((string)databaseName).GetContainer((string)containerName).GetItemQueryIterator<T>(request.SqlQuery);
        }
    }
}