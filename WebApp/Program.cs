using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Override the port to listen on if passed via an environment variable (e.g. for App Service).
                    // See https://github.com/Azure/app-service-linux-docs/blob/master/app_service_linux_vnet_integration.md.
                    if (int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port))
                    {
                        webBuilder.UseUrls($"http://*:{port}");
                    }
                    webBuilder.UseStartup<Startup>();
                });
    }
}
