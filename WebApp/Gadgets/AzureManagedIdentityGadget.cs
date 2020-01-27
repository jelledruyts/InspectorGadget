using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using InspectorGadget.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace InspectorGadget.WebApp.Gadgets
{
    public class AzureManagedIdentityGadget : GadgetBase<AzureManagedIdentityGadget.Request, AzureManagedIdentityGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string Resource { get; set; }
        }

        public class Result
        {
            public string AccessToken { get; set; }
        }

        public AzureManagedIdentityGadget(IHttpClientFactory httpClientFactory, IUrlHelper url)
            : base(httpClientFactory, url, nameof(ApiController.AzureManagedIdentity))
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            var httpClient = this.HttpClientFactory.CreateClient();
            var msiEndpoint = Environment.GetEnvironmentVariable("MSI_ENDPOINT");
            var msiSecret = Environment.GetEnvironmentVariable("MSI_SECRET");
            var result = new Result();
            if (!string.IsNullOrWhiteSpace(msiEndpoint) && !string.IsNullOrWhiteSpace(msiSecret))
            {
                // Running on App Service, use the corresponding endpoint and header.
                var requestUrl = msiEndpoint + "?api-version=2017-09-01&resource=" + HttpUtility.UrlEncode(request.Resource);
                httpClient.DefaultRequestHeaders.Add("Secret", msiSecret);
                result.AccessToken = await httpClient.GetStringAsync(requestUrl);
            }
            else
            {
                // See https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/how-to-use-vm-token#get-a-token-using-c
                var requestUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=" + HttpUtility.UrlEncode(request.Resource);
                httpClient.DefaultRequestHeaders.Add("Metadata", "true");
                result.AccessToken = await httpClient.GetStringAsync(requestUrl);
            }
            return result;
        }
    }
}