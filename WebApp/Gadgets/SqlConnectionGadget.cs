using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace InspectorGadget.WebApp.Gadgets
{
    public class SqlConnectionGadget
    {
        public class Request
        {
            public string SqlConnectionString { get; set; }
            public string SqlQuery { get; set; }
        }

        public class Response
        {
            public string Result { get; set; }
        }

        public static async Task<Response> ExecuteAsync(Request request)
        {
            using (var connection = new SqlConnection(request.SqlConnectionString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = request.SqlQuery;
                var result = await command.ExecuteScalarAsync();
                return new Response
                {
                    Result = result?.ToString()
                };
            }
        }
    }
}