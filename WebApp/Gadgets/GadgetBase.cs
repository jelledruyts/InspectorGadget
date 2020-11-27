using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Gadgets
{
    public abstract class GadgetBase<TRequest, TResult> where TRequest : GadgetRequest
    {
        protected ILogger Logger { get; }
        protected IHttpClientFactory HttpClientFactory { get; }
        protected string RelativeUrl { get; }
        protected bool IsDisabled { get; }

        protected GadgetBase(ILogger logger, IHttpClientFactory httpClientFactory, string relativeUrl, bool isDisabled)
        {
            this.Logger = logger;
            this.HttpClientFactory = httpClientFactory;
            this.RelativeUrl = relativeUrl;
            this.IsDisabled = isDisabled;
        }

        public async Task<GadgetResponse<TResult>> ExecuteAsync(TRequest request)
        {
            this.Logger.LogDebug("Executing Gadget base functionality");
            var response = new GadgetResponse<TResult>();
            if (this.IsDisabled)
            {
                response.Error = "This gadget is disabled";
            }
            else
            {
                try
                {
                    this.Logger.LogDebug("Executing Gadget core functionality");
                    response.Result = await ExecuteCoreAsync(request);
                    this.Logger.LogDebug("Executed Gadget core functionality");
                }
                catch (Exception exc)
                {
                    this.Logger.LogError(exc, "Exception while executing Gadget core functionality");
                    response.Error = exc.ToString();
                }
            }
            response.ChainedResponse = await this.PerformCallChainAsync(request);
            response.TimeCompleted = DateTimeOffset.UtcNow;
            this.Logger.LogDebug("Executed Gadget base functionality");
            return response;
        }

        protected abstract Task<TResult> ExecuteCoreAsync(TRequest request);

        private async Task<GadgetResponse<TResult>> PerformCallChainAsync(GadgetRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CallChainUrls))
            {
                return null;
            }
            this.Logger.LogDebug("Executing Gadget call chain");
            var originalCallChainUrls = request.CallChainUrls;
            try
            {
                var callChainUrls = request.CallChainUrls.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var nextCallChainUrl = new Uri(new Uri(callChainUrls[0]), this.RelativeUrl);
                // Temporarily set the shortened call chain URLs and send it off to the next hop.
                request.CallChainUrls = string.Join(' ', callChainUrls, 1, callChainUrls.Length - 1);
                this.Logger.LogInformation("Executing Gadget call chain to next hop {NextCallChainUrl}", nextCallChainUrl);
                var callChainClient = this.HttpClientFactory.CreateClient();
                var callChainResponse = await callChainClient.PostAsJsonAsync(nextCallChainUrl, request);
                callChainResponse.EnsureSuccessStatusCode();
                return await callChainResponse.Content.ReadAsAsync<GadgetResponse<TResult>>();
            }
            catch (Exception exc)
            {
                this.Logger.LogError(exc, "Exception while executing Gadget call chain");
                return new GadgetResponse<TResult> { Error = exc.ToString(), TimeCompleted = DateTimeOffset.UtcNow };
            }
            finally
            {
                // Restore the original call chain URLs to ensure the current request is still correctly displayed.
                request.CallChainUrls = originalCallChainUrls;
                this.Logger.LogDebug("Executed Gadget call chain");
            }
        }
    }
}