using System.Text.Json.Serialization;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InspectorGadget.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Enforce use of TLS 1.2.
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            services.AddSingleton<AppSettings>();
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
            services.AddRazorPages(options =>
            {
                // Disable ASP.NET Core antiforgery (as it depends on Data Protection keys which are environment-dependent and this container should be as portable as possible).
                options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            }).AddRazorRuntimeCompilation();
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddHttpClient();
            services.AddHealthChecks()
                .AddCheck<ConfigurableHealthCheck>(nameof(ConfigurableHealthCheck));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSettings appSettings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHealthChecks(ConfigurableHealthCheck.HealthCheckPath);
            });

            ConfigurableHealthCheck.Configure(appSettings.DefaultHealthCheckMode, appSettings.DefaultHealthCheckFailNumberOfTimes);
        }
    }
}
