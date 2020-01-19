using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class DnsLookupModel : PageModel
    {
        [BindProperty]
        public string Host { get; set; }

        public IPHostEntry DnsEntry { get; set; }
        public Exception Exception { get; set; }

        public async Task OnPost()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.Host))
                {
                    this.DnsEntry = await Dns.GetHostEntryAsync(this.Host);
                }
            }
            catch (Exception exc)
            {
                this.Exception = exc;
            }
        }
    }
}