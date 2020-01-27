using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace InspectorGadget.WebApp.Gadgets
{
    public class DnsLookupGadget : GadgetBase<DnsLookupGadget.Request, DnsLookupGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string Host { get; set; }
        }

        public class Result
        {
            public string HostName { get; set; }
            public IList<string> Addresses { get; set; }
            public IList<string> Aliases { get; set; }
        }

        public DnsLookupGadget(IHttpClientFactory httpClientFactory, IUrlHelper url)
            : base(httpClientFactory, url, nameof(ApiController.DnsLookup))
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            var hostEntry = await Dns.GetHostEntryAsync(request.Host);
            return new Result
            {
                HostName = hostEntry.HostName,
                Addresses = hostEntry.AddressList.Select(a => a.ToString()).ToArray(),
                Aliases = hostEntry.Aliases
            };
        }
    }
}