using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Gadgets
{
    public class AzureManagedIdentityGadget : GadgetBase<AzureManagedIdentityGadget.Request, AzureManagedIdentityGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string Scopes { get; set; }
            public string AzureManagedIdentityClientId { get; set; }
        }

        public class Result
        {
            public string AccessToken { get; set; }
            public DateTimeOffset ExpiresOn { get; set; }
        }

        public AzureManagedIdentityGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url, AppSettings appSettings)
            : base(logger, httpClientFactory, url.GetRelativeApiUrl(nameof(ApiController.AzureManagedIdentity)), appSettings.DisableAzureManagedIdentity)
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Acquiring token using Azure Managed Identity for Scopes \"{Scopes}\" using Client ID \"{ClientId}\"", request.Scopes, request.AzureManagedIdentityClientId);
            var scopes = request.Scopes == null ? Array.Empty<string>() : request.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            // If AzureManagedIdentityClientId is requested, that indicates the User-Assigned Managed Identity to use; if omitted the System-Assigned Managed Identity will be used.
            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = request.AzureManagedIdentityClientId });
            var authenticationResult = await credential.GetTokenAsync(new TokenRequestContext(scopes));
            return new Result
            {
                AccessToken = authenticationResult.Token,
                ExpiresOn = authenticationResult.ExpiresOn
            };
        }
    }
}