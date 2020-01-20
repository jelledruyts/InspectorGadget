using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace InspectorGadget.WebApp.Gadgets
{
    public class DnsLookupGadget
    {
        public class Request
        {
            public string Host { get; set; }
        }

        public class Response
        {
            public string HostName { get; set; }
            public IList<string> Addresses { get; set; }
            public IList<string> Aliases { get; set; }
        }

        public static async Task<Response> ExecuteAsync(Request request)
        {
            var hostEntry = await Dns.GetHostEntryAsync(request.Host);
            return new Response
            {
                HostName = hostEntry.HostName,
                Addresses = hostEntry.AddressList.Select(a => a.ToString()).ToArray(),
                Aliases = hostEntry.Aliases
            };
        }
    }
}