using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace InspectorGadget.WebApp.Gadgets
{
    public abstract class GadgetBase<TRequest, TResult> where TRequest : GadgetRequest
    {
        protected IHttpClientFactory HttpClientFactory { get; }
        protected string RelativeUrl { get; }

        protected GadgetBase(IHttpClientFactory httpClientFactory, IUrlHelper url, string apiActionName)
        {
            this.HttpClientFactory = httpClientFactory;
            this.RelativeUrl = url.Action(apiActionName, "Api");
        }

        public async Task<GadgetResponse<TResult>> ExecuteAsync(TRequest request)
        {
            var response = new GadgetResponse<TResult>();
            try
            {
                response.Result = await ExecuteCoreAsync(request);
            }
            catch (Exception exc)
            {
                response.Error = exc.ToString();
            }
            response.ChainedResponse = await this.PerformCallChainAsync(request);
            response.TimeCompleted = DateTimeOffset.UtcNow;
            return response;
        }

        protected abstract Task<TResult> ExecuteCoreAsync(TRequest request);

        private async Task<GadgetResponse<TResult>> PerformCallChainAsync(GadgetRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CallChainUrls))
            {
                return null;
            }
            var originalCallChainUrls = request.CallChainUrls;
            try
            {
                var callChainUrls = request.CallChainUrls.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var nextCallChainUrl = new Uri(new Uri(callChainUrls[0]), this.RelativeUrl);
                // Temporarily set the shortened call chain URLs and send it off to the next hop.
                request.CallChainUrls = string.Join(' ', callChainUrls, 1, callChainUrls.Length - 1);
                var callChainClient = this.HttpClientFactory.CreateClient();
                var callChainResponse = await callChainClient.PostAsJsonAsync(nextCallChainUrl, request);
                callChainResponse.EnsureSuccessStatusCode();
                return await callChainResponse.Content.ReadAsAsync<GadgetResponse<TResult>>();
            }
            catch (Exception exc)
            {
                return new GadgetResponse<TResult> { Error = exc.ToString(), TimeCompleted = DateTimeOffset.UtcNow };
            }
            finally
            {
                // Restore the original call chain URLs to ensure the current request is still correctly displayed.
                request.CallChainUrls = originalCallChainUrls;
            }
        }
    }
}