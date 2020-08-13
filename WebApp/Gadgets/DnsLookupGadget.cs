using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        public DnsLookupGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url)
            : base(logger, httpClientFactory, url, nameof(ApiController.DnsLookup))
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Resolving DNS for Host {Host}", request.Host);
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