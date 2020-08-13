using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace InspectorGadget.WebApp.Gadgets
{
    public class HttpRequestGadget : GadgetBase<HttpRequestGadget.Request, HttpRequestGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string RequestUrl { get; set; }
            public string RequestHostName { get; set; }
        }

        public class Result
        {
            public string ResponseBody { get; set; }
        }

        public HttpRequestGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url)
            : base(logger, httpClientFactory, url, nameof(ApiController.HttpRequest))
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Executing HTTP Request for RequestUrl {RequestUrl} and RequestHostName {RequestHostName}", request.RequestUrl, request.RequestHostName);
            var httpClient = this.HttpClientFactory.CreateClient();
            if (!string.IsNullOrWhiteSpace(request.RequestHostName))
            {
                httpClient.DefaultRequestHeaders.Add(HeaderNames.Host, request.RequestHostName);
            }
            return new Result { ResponseBody = await httpClient.GetStringAsync(request.RequestUrl) };
        }
    }
}