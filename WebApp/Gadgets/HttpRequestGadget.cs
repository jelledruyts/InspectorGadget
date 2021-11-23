using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Infrastructure;
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
            public bool IgnoreServerCertificateErrors { get; set; }
        }

        public class Result
        {
            public string ResponseBody { get; set; }
        }

        public HttpRequestGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url, AppSettings appSettings)
            : base(logger, httpClientFactory, url.GetRelativeApiUrl(nameof(ApiController.HttpRequest)), appSettings.DisableHttpRequest)
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Executing HTTP Request for RequestUrl {RequestUrl} and RequestHostName {RequestHostName}", request.RequestUrl, request.RequestHostName);
            var httpClient = request.IgnoreServerCertificateErrors ? this.HttpClientFactory.CreateClient(Startup.HttpClientNameAcceptAnyServerCertificate) : this.HttpClientFactory.CreateClient();
            if (!string.IsNullOrWhiteSpace(request.RequestHostName))
            {
                httpClient.DefaultRequestHeaders.Add(HeaderNames.Host, request.RequestHostName);
            }
            return new Result { ResponseBody = await httpClient.GetStringAsync(request.RequestUrl) };
        }
    }
}