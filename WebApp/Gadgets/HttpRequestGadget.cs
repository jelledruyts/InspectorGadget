using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace InspectorGadget.WebApp.Gadgets
{
    public class HttpRequestGadget
    {
        public class Request
        {
            public string RequestUrl { get; set; }
            public string RequestHostName { get; set; }
        }

        public class Response
        {
            public string ResponseBody { get; set; }
        }

        public static async Task<Response> ExecuteAsync(Request request, IHttpClientFactory httpClientFactory)
        {
            var httpClient = httpClientFactory.CreateClient();
            if (!string.IsNullOrWhiteSpace(request.RequestHostName))
            {
                httpClient.DefaultRequestHeaders.Add(HeaderNames.Host, request.RequestHostName);
            }
            var responseBody = await httpClient.GetStringAsync(request.RequestUrl);
            return new Response
            {
                ResponseBody = responseBody
            };
        }
    }
}