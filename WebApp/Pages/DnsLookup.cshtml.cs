using System;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class DnsLookupModel : PageModel
    {
        [BindProperty]
        public DnsLookupGadget.Request GadgetRequest { get; set; } = new DnsLookupGadget.Request();

        public DnsLookupGadget.Response GadgetResponse { get; set; }
        public Exception Exception { get; set; }

        public async Task OnPost()
        {
            try
            {
                this.GadgetResponse = await DnsLookupGadget.ExecuteAsync(this.GadgetRequest);
            }
            catch (Exception exc)
            {
                this.Exception = exc;
            }
        }
    }
}