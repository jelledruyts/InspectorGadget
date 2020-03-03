using System;
using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;

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

        public AzureManagedIdentityGadget(IHttpClientFactory httpClientFactory, IUrlHelper url)
            : base(httpClientFactory, url, nameof(ApiController.AzureManagedIdentity))
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
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