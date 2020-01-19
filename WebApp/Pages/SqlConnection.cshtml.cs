using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace InspectorGadget.WebApp.Pages
{
    public class SqlConnectionModel : PageModel
    {
        [BindProperty]
        public string SqlConnectionString { get; set; }
        [BindProperty]
        public string SqlQuery { get; set; } = "SELECT CONNECTIONPROPERTY('client_net_address')";

        public string Result { get; set; }
        public Exception Exception { get; set; }


        public async Task OnPost()
        {
            try
            {
                using (var connection = new SqlConnection(this.SqlConnectionString))
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = this.SqlQuery;
                    var result = await command.ExecuteScalarAsync();
                    this.Result = result?.ToString();
                }
            }
            catch (Exception exc)
            {
                this.Exception = exc;
            }
        }
    }
}