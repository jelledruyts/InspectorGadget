using System;
using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;

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
            public DateTimeOffset ExpiresOn { get; set; }
            public string Resource { get; set; }
            public string TokenType { get; set; }
        }

        public AzureManagedIdentityGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url, AppSettings appSettings)
            : base(logger, httpClientFactory, url.GetRelativeApiUrl(nameof(ApiController.AzureManagedIdentity)), appSettings.DisableAzureManagedIdentity)
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Acquiring token using Azure Managed Identity for Resource {Resource}", request.Resource);
            var authenticationResult = await new AzureServiceTokenProvider().GetAuthenticationResultAsync(request.Resource);
            return new Result
            {
                AccessToken = authenticationResult.AccessToken,
                ExpiresOn = authenticationResult.ExpiresOn,
                Resource = authenticationResult.Resource,
                TokenType = authenticationResult.TokenType
            };
        }
    }
}