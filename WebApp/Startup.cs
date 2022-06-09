using System;
using System.Net.Http;
using System.Text.Json.Serialization;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InspectorGadget.WebApp
{
    public class Startup
    {
        public const string HttpClientNameAcceptAnyServerCertificate = nameof(HttpClientNameAcceptAnyServerCertificate);

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
            services.AddHttpClient(HttpClientNameAcceptAnyServerCertificate).ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator });
            services.AddHealthChecks()
                .AddCheck<ConfigurableHealthCheck>(nameof(ConfigurableHealthCheck));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSettings appSettings)
        {
            if (!string.IsNullOrWhiteSpace(appSettings.PathBase))
            {
                try
                {
                    app.UsePathBase(appSettings.PathBase);
                    // Note: using the path base middleware as above doesn't work if a proxy actually strips the incoming path:
                    // the middleware only sets the request path base if the incoming request does in fact start with it.
                    // E.g. when a proxy forwards "/app/foo" to "/foo" and the middleware is configured with "/app" as the
                    // path base, then it will not set the request path base to "/app" because the incoming request is seen as
                    // "/foo" and does not start with "/app". This will result in incorrect relative URL's etc.
                    // To cover for this scenario as well, we also explicitly set the configured path base on each request directly,
                    // regardless of whether or not the incoming request had it.
                    // See https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#work-with-path-base-and-proxies-that-change-the-request-path
                    // and https://github.com/dotnet/aspnetcore/blob/2b7e994b8a304700a09617ffc5052f0d943bbcba/src/Http/Http.Abstractions/src/Extensions/UsePathBaseMiddleware.cs#L54.
                    var pathBase = new PathString(appSettings.PathBase);
                    app.Use((context, next) =>
                    {
                        context.Request.PathBase = pathBase;
                        return next.Invoke();
                    });
                }
                catch (Exception exc)
                {
                    appSettings.ErrorMessage = $"Cannot apply \"PathBase\" configuration setting: {exc.Message}";
                }
            }

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
