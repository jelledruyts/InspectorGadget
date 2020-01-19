using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class AzureManagedIdentityModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public string Resource { get; set; } = "https://management.azure.com/";

        public string AccessToken { get; set; }
        public Exception Exception { get; set; }

        public AzureManagedIdentityModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnPost()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.Resource))
                {
                    var msiEndpoint = Environment.GetEnvironmentVariable("MSI_ENDPOINT");
                    var msiSecret = Environment.GetEnvironmentVariable("MSI_SECRET");
                    if (!string.IsNullOrWhiteSpace(msiEndpoint) && !string.IsNullOrWhiteSpace(msiSecret))
                    {
                        // Running on App Service, use the corresponding endpoint and header.
                        var requestUrl = msiEndpoint + "?api-version=2017-09-01&resource=" + HttpUtility.UrlEncode(this.Resource);
                        var httpClient = this.httpClientFactory.CreateClient();
                        httpClient.DefaultRequestHeaders.Add("Secret", msiSecret);
                        this.AccessToken = await httpClient.GetStringAsync(requestUrl);
                    }
                    else
                    {
                        // See https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/how-to-use-vm-token#get-a-token-using-c
                        var requestUrl = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=" + HttpUtility.UrlEncode(this.Resource);
                        var httpClient = this.httpClientFactory.CreateClient();
                        httpClient.DefaultRequestHeaders.Add("Metadata", "true");
                        this.AccessToken = await httpClient.GetStringAsync(requestUrl);
                    }
                }
            }
            catch (Exception exc)
            {
                this.Exception = exc;
            }
        }
    }
}