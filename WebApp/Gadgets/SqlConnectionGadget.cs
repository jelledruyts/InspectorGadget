using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace InspectorGadget.WebApp.Gadgets
{
    public class SqlConnectionGadget
    {
        public class Request : GadgetRequest<Request>
        {
            public string SqlConnectionString { get; set; }
            public string SqlQuery { get; set; }

            public override Request Clone()
            {
                return new Request { SqlConnectionString = this.SqlConnectionString, SqlQuery = this.SqlQuery };
            }
        }

        public class Response : GadgetResponse<Request, Response>
        {
            public string Result { get; set; }
        }

        public static async Task<Response> ExecuteAsync(Request request, string relativeUrl, IHttpClientFactory httpClientFactory)
        {
            var response = new Response { Request = request };
            try
            {
                using (var connection = new SqlConnection(request.SqlConnectionString))
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = request.SqlQuery;
                    var result = await command.ExecuteScalarAsync();
                    response.Result = result?.ToString();
                }
            }
            catch (Exception exc)
            {
                response.Error = exc.ToString();
            }
            response.ChainedResponse = await request.PerformCallChainAsync<Response>(httpClientFactory, relativeUrl);
            response.TimeCompleted = DateTimeOffset.UtcNow;
            return response;
        }
    }
}