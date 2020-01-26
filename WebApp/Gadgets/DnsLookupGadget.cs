using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InspectorGadget.WebApp.Gadgets
{
    public class DnsLookupGadget
    {
        public class Request : GadgetRequest<Request>
        {
            public string Host { get; set; }

            public override Request Clone()
            {
                return new Request { Host = this.Host };
            }
        }

        public class Response : GadgetResponse<Request, Response>
        {
            public string HostName { get; set; }
            public IList<string> Addresses { get; set; }
            public IList<string> Aliases { get; set; }
        }

        public static async Task<Response> ExecuteAsync(Request request, string relativeUrl, IHttpClientFactory httpClientFactory)
        {
            var response = new Response { Request = request };
            try
            {
                var hostEntry = await Dns.GetHostEntryAsync(request.Host);
                response.HostName = hostEntry.HostName;
                response.Addresses = hostEntry.AddressList.Select(a => a.ToString()).ToArray();
                response.Aliases = hostEntry.Aliases;
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