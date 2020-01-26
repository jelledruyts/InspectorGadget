using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace InspectorGadget.WebApp.Gadgets
{
    public abstract class GadgetRequest<TRequest> where TRequest : GadgetRequest<TRequest>
    {
        public string CallChainUrls { get; set; }

        public abstract TRequest Clone();

        public async Task<TResponse> PerformCallChainAsync<TResponse>(IHttpClientFactory httpClientFactory, string relativeUrl) where TResponse : GadgetResponse<TRequest, TResponse>, new()
        {
            if (string.IsNullOrWhiteSpace(this.CallChainUrls))
            {
                return null;
            }
            var chainedRequest = this.Clone();
            var callChainUrls = this.CallChainUrls.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (callChainUrls.Length > 1)
            {
                chainedRequest.CallChainUrls = string.Join(' ', callChainUrls, 1, callChainUrls.Length - 1);
            }
            try
            {
                var callChainClient = httpClientFactory.CreateClient();
                var nextCallChainUrl = new Uri(new Uri(callChainUrls[0]), relativeUrl);
                var callChainResponse = await callChainClient.PostAsJsonAsync(nextCallChainUrl, chainedRequest);
                callChainResponse.EnsureSuccessStatusCode();
                return await callChainResponse.Content.ReadAsAsync<TResponse>();
            }
            catch (Exception exc)
            {
                return new TResponse { Request = chainedRequest, Error = exc.ToString(), TimeCompleted = DateTimeOffset.UtcNow };
            }
        }
    }
}