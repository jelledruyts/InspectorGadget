using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Models
{
    public class InspectorInfo
    {
        public IList<KeyValuePair<string, string>> Request { get; set; }
        public IList<KeyValuePair<string, string>> HttpHeaders { get; set; }
        public IList<KeyValuePair<string, string>> Identity { get; set; }
        public IList<KeyValuePair<string, string>> Application { get; set; }
        public IList<KeyValuePair<string, string>> Configuration { get; set; }
        public IList<KeyValuePair<string, string>> Environment { get; set; }
        public IList<KeyValuePair<string, string>> System { get; set; }

        public static InspectorInfo Create(IWebHostEnvironment environment, IConfiguration configuration, HttpRequest request)
        {
            var environmentInfo = GetEnvironmentInfo();
            return new InspectorInfo
            {
                Request = GetRequestInfo(request),
                HttpHeaders = GetHttpHeadersInfo(request),
                Identity = GetIdentityInfo(request.HttpContext.User),
                Application = GetApplicationInfo(environment),
                Configuration = GetConfigurationInfo(configuration, environmentInfo),
                Environment = environmentInfo,
                System = GetSystemInfo()
            };
        }

        private static IList<KeyValuePair<string, string>> GetRequestInfo(HttpRequest request)
        {
            var info = new List<KeyValuePair<string, string>>();
            info.Add(new KeyValuePair<string, string>("URL", request.GetDisplayUrl()));
            info.Add(new KeyValuePair<string, string>("HTTP Method", request.Method));
            info.Add(new KeyValuePair<string, string>("Is HTTPS", request.IsHttps.ToString()));
            info.Add(new KeyValuePair<string, string>("Client Certificate Serial Number", request.HttpContext.Connection.ClientCertificate?.SerialNumber));
            info.Add(new KeyValuePair<string, string>("Local IP Address", request.HttpContext.Connection.LocalIpAddress.ToString()));
            info.Add(new KeyValuePair<string, string>("Local Port", request.HttpContext.Connection.LocalPort.ToString()));
            info.Add(new KeyValuePair<string, string>("Remote IP Address", request.HttpContext.Connection.RemoteIpAddress.ToString()));
            info.Add(new KeyValuePair<string, string>("Remote Port", request.HttpContext.Connection.RemotePort.ToString()));
            return info;
        }

        private static IList<KeyValuePair<string, string>> GetHttpHeadersInfo(HttpRequest request)
        {
            var info = new List<KeyValuePair<string, string>>();
            foreach (var item in request.Headers.OrderBy(k => k.Key))
            {
                foreach (var value in request.Headers[item.Key])
                {
                    info.Add(new KeyValuePair<string, string>(item.Key, value));
                }
            }
            return info;
        }

        private static IList<KeyValuePair<string, string>> GetIdentityInfo(ClaimsPrincipal user)
        {
            var info = new List<KeyValuePair<string, string>>();
            var identity = (ClaimsIdentity)user.Identity;
            info.Add(new KeyValuePair<string, string>("User Name", user.Identity.Name));
            info.Add(new KeyValuePair<string, string>("User Is Authenticated", user.Identity.IsAuthenticated.ToString()));
            info.Add(new KeyValuePair<string, string>("User Authentication Type", user.Identity.AuthenticationType));
            foreach (var claim in identity.Claims.OrderBy(c => c.Type))
            {
                info.Add(new KeyValuePair<string, string>(claim.Type, claim.Value));
            }
            return info;
        }

        private static IList<KeyValuePair<string, string>> GetApplicationInfo(IWebHostEnvironment environment)
        {
            var info = new List<KeyValuePair<string, string>>();
            info.Add(new KeyValuePair<string, string>("Application Name", environment.ApplicationName));
            info.Add(new KeyValuePair<string, string>("Content Root Path", environment.ContentRootPath));
            info.Add(new KeyValuePair<string, string>("Web Root Path", environment.WebRootPath));
            info.Add(new KeyValuePair<string, string>("Environment Name", environment.EnvironmentName));
            return info;
        }

        private static IList<KeyValuePair<string, string>> GetConfigurationInfo(IConfiguration configuration, IList<KeyValuePair<string, string>> itemsToExclude)
        {
            var info = new List<KeyValuePair<string, string>>();
            foreach (var item in configuration.AsEnumerable().OrderBy(k => k.Key))
            {
                if (itemsToExclude == null || !itemsToExclude.Any(i => i.Key == item.Key && i.Value == item.Value))
                {
                    info.Add(new KeyValuePair<string, string>(item.Key, item.Value));
                }
            }
            return info;
        }

        private static IList<KeyValuePair<string, string>> GetEnvironmentInfo()
        {
            var info = new List<KeyValuePair<string, string>>();
            foreach (var item in global::System.Environment.GetEnvironmentVariables().Cast<DictionaryEntry>().OrderBy(e => e.Key))
            {
                info.Add(new KeyValuePair<string, string>(item.Key?.ToString(), item.Value?.ToString()));
            }
            return info;
        }

        private static IList<KeyValuePair<string, string>> GetSystemInfo()
        {
            var info = new List<KeyValuePair<string, string>>();
            info.Add(new KeyValuePair<string, string>("Machine Name", global::System.Environment.MachineName));
            info.Add(new KeyValuePair<string, string>("64-bit OS", global::System.Environment.Is64BitOperatingSystem.ToString()));
            info.Add(new KeyValuePair<string, string>("64-bit Process", global::System.Environment.Is64BitProcess.ToString()));
            info.Add(new KeyValuePair<string, string>("OS Version", global::System.Environment.OSVersion.ToString()));
            info.Add(new KeyValuePair<string, string>("Processor Count", global::System.Environment.ProcessorCount.ToString()));
            info.Add(new KeyValuePair<string, string>("CLR Version", global::System.Environment.Version.ToString()));
            info.Add(new KeyValuePair<string, string>("Logged On User Domain", global::System.Environment.UserDomainName));
            info.Add(new KeyValuePair<string, string>("Logged On User Name", global::System.Environment.UserName));
            info.Add(new KeyValuePair<string, string>("Garbage Collection Mode", GCSettings.IsServerGC ? "Server" : "Workstation"));
            info.Add(new KeyValuePair<string, string>("System Time", Format(DateTimeOffset.UtcNow)));
            info.Add(new KeyValuePair<string, string>("Process Uptime", GetProcessUptime().ToString()));
            info.Add(new KeyValuePair<string, string>("Process Start Time", Format(DateTime.UtcNow - GetProcessUptime())));
            return info;
        }

        private static TimeSpan GetProcessUptime()
        {
            return (DateTime.Now - Process.GetCurrentProcess().StartTime);
        }

        private static string Format(DateTime value)
        {
            return value.ToString("u") + " (UTC)";
        }

        private static string Format(DateTimeOffset value)
        {
            return value.ToString("u") + " (UTC)";
        }
    }
}