using System;
using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;

namespace InspectorGadget.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GadgetController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        public GadgetController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<object> DnsLookup([FromBody]DnsLookupGadget.Request request)
        {
            try
            {
                return await DnsLookupGadget.ExecuteAsync(request);
            }
            catch (Exception exc)
            {
                return Problem(exc.ToString());
            }
        }

        [HttpPost]
        public async Task<object> HttpRequest([FromBody]HttpRequestGadget.Request request)
        {
            try
            {
                return await HttpRequestGadget.ExecuteAsync(request, this.httpClientFactory);
            }
            catch (Exception exc)
            {
                return Problem(exc.ToString());
            }
        }
        
        [HttpPost]
        public async Task<object> SqlConnection([FromBody]SqlConnectionGadget.Request request)
        {
            try
            {
                return await SqlConnectionGadget.ExecuteAsync(request);
            }
            catch (Exception exc)
            {
                return Problem(exc.ToString());
            }
        }
        
        [HttpPost]
        public async Task<object> AzureManagedIdentity([FromBody]AzureManagedIdentityGadget.Request request)
        {
            try
            {
                return await AzureManagedIdentityGadget.ExecuteAsync(request, this.httpClientFactory);
            }
            catch (Exception exc)
            {
                return Problem(exc.ToString());
            }
        }
    }
}