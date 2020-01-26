using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace InspectorGadget.WebApp.Gadgets
{
    public class HttpRequestGadget
    {
        public class Request : GadgetRequest<Request>
        {
            public string RequestUrl { get; set; }
            public string RequestHostName { get; set; }

            public override Request Clone()
            {
                return new Request { RequestUrl = this.RequestUrl, RequestHostName = this.RequestHostName };
            }
        }

        public class Response : GadgetResponse<Request, Response>
        {
            public string ResponseBody { get; set; }
        }

        public static async Task<Response> ExecuteAsync(Request request, string relativeUrl, IHttpClientFactory httpClientFactory)
        {
            var response = new Response { Request = request };
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                if (!string.IsNullOrWhiteSpace(request.RequestHostName))
                {
                    httpClient.DefaultRequestHeaders.Add(HeaderNames.Host, request.RequestHostName);
                }
                response.ResponseBody = await httpClient.GetStringAsync(request.RequestUrl);
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