using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace InspectorGadget.WebApp.Gadgets
{
    public class AzureManagedIdentityGadget
    {
        public class Request : GadgetRequest<Request>
        {
            public string Resource { get; set; }

            public override Request Clone()
            {
                return new Request { Resource = this.Resource };
            }
        }

        public class Response : GadgetResponse<Request, Response>
        {
            public string AccessToken { get; set; }
        }

        public static async Task<Response> ExecuteAsync(Request request, string relativeUrl, IHttpClientFactory httpClientFactory)
        {
            var response = new Response { Request = request };
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                var msiEndpoint = Environment.GetEnvironmentVariable("MSI_ENDPOINT");
                var msiSecret = Environment.GetEnvironmentVariable("MSI_SECRET");
                if (!string.IsNullOrWhiteSpace(msiEndpoint) && !string.IsNullOrWhiteSpace(msiSecret))
                {
                    // Running on App Service, use the corresponding endpoint and header.
                    var requestUrl = msiEndpoint + "?api-version=2017-09-01&resource=" + HttpUtility.UrlEncode(request.Resource);
                    httpClient.DefaultRequestHeaders.Add("Secret", msiSecret);
                    response.AccessToken = await httpClient.GetStringAsync(requestUrl);
                }
                else
                {
                    // See https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/how-to-use-vm-token#get-a-token-using-c
                    var requestUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=" + HttpUtility.UrlEncode(request.Resource);
                    httpClient.DefaultRequestHeaders.Add("Metadata", "true");
                    response.AccessToken = await httpClient.GetStringAsync(requestUrl);
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