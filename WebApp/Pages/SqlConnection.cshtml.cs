using System;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Gadgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InspectorGadget.WebApp.Pages
{
    public class SqlConnectionModel : PageModel
    {
        [BindProperty]
        public SqlConnectionGadget.Request GadgetRequest { get; set; } = new SqlConnectionGadget.Request { SqlQuery = "SELECT CONNECTIONPROPERTY('client_net_address')" };

        public SqlConnectionGadget.Response GadgetResponse { get; set; }
        public Exception Exception { get; set; }


        public async Task OnPost()
        {
            try
            {
                this.GadgetResponse = await SqlConnectionGadget.ExecuteAsync(this.GadgetRequest);
            }
            catch (Exception exc)
            {
                this.Exception = exc;
            }
        }
    }
}