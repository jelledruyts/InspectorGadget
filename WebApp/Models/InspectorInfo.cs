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
        public IList<InspectorValue> Request { get; set; }
        public IList<InspectorValue> HttpHeaders { get; set; }
        public IList<InspectorValue> Identity { get; set; }
        public IList<InspectorValue> Application { get; set; }
        public IList<InspectorValue> Configuration { get; set; }
        public IList<InspectorValue> Environment { get; set; }
        public IList<InspectorValue> System { get; set; }

        public static InspectorInfo Create(IWebHostEnvironment environment, IConfiguration configuration, HttpRequest request)
        {
            var environmentInfo = GetEnvironmentInfo();
            return new InspectorInfo
            {
                Request = GetRequestInfo(request),
                HttpHeaders = GetHttpHeadersInfo(request),
                Identity = GetIdentityInfo(request.HttpContext.User),
                Configuration = GetConfigurationInfo(configuration, environmentInfo),
                Environment = environmentInfo,
                Application = GetApplicationInfo(environment),
                System = GetSystemInfo()
            };
        }

        private static IList<InspectorValue> GetRequestInfo(HttpRequest request)
        {
            var info = new List<InspectorValue>();
            info.Add(new InspectorValue("request-url", "URL", request.GetDisplayUrl()));
            info.Add(new InspectorValue("request-method", "HTTP Method", request.Method));
            info.Add(new InspectorValue("request-ishttps", "Is HTTPS", request.IsHttps.ToString()));
            info.Add(new InspectorValue("clientcertificate-serialnumber", "Client Certificate Serial Number", request.HttpContext.Connection.ClientCertificate?.SerialNumber));
            info.Add(new InspectorValue("local-ipaddress", "Local IP Address", request.HttpContext.Connection.LocalIpAddress.ToString()));
            info.Add(new InspectorValue("local-port", "Local Port", request.HttpContext.Connection.LocalPort.ToString()));
            info.Add(new InspectorValue("remote-ipaddress", "Remote IP Address", request.HttpContext.Connection.RemoteIpAddress.ToString()));
            info.Add(new InspectorValue("remote-port", "Remote Port", request.HttpContext.Connection.RemotePort.ToString()));
            return info;
        }

        private static IList<InspectorValue> GetHttpHeadersInfo(HttpRequest request)
        {
            var info = new List<InspectorValue>();
            foreach (var item in request.Headers.OrderBy(k => k.Key))
            {
                foreach (var value in request.Headers[item.Key])
                {
                    info.Add(new InspectorValue(item.Key, item.Key, value));
                }
            }
            return info;
        }

        private static IList<InspectorValue> GetIdentityInfo(ClaimsPrincipal user)
        {
            var info = new List<InspectorValue>();
            var identity = (ClaimsIdentity)user.Identity;
            info.Add(new InspectorValue("user-name", "User Name", user.Identity.Name));
            info.Add(new InspectorValue("user-isauthenticated", "User Is Authenticated", user.Identity.IsAuthenticated.ToString()));
            info.Add(new InspectorValue("user-authenticationtype", "User Authentication Type", user.Identity.AuthenticationType));
            foreach (var claim in identity.Claims.OrderBy(c => c.Type))
            {
                info.Add(new InspectorValue("user-claim-" + claim.Type, claim.Type, claim.Value));
            }
            return info;
        }

        private static IList<InspectorValue> GetConfigurationInfo(IConfiguration configuration, IList<InspectorValue> itemsToExclude)
        {
            var info = new List<InspectorValue>();
            foreach (var item in configuration.AsEnumerable().OrderBy(k => k.Key))
            {
                if (itemsToExclude == null || !itemsToExclude.Any(i => i.Key == item.Key && i.Value == item.Value))
                {
                    info.Add(new InspectorValue(item.Key, item.Key, item.Value));
                }
            }
            return info;
        }

        private static IList<InspectorValue> GetEnvironmentInfo()
        {
            var info = new List<InspectorValue>();
            foreach (var item in global::System.Environment.GetEnvironmentVariables().Cast<DictionaryEntry>().OrderBy(e => e.Key))
            {
                info.Add(new InspectorValue(item.Key?.ToString(), item.Key?.ToString(), item.Value?.ToString()));
            }
            return info;
        }

        private static IList<InspectorValue> GetApplicationInfo(IWebHostEnvironment environment)
        {
            var info = new List<InspectorValue>();
            info.Add(new InspectorValue("application-name", "Application Name", environment.ApplicationName));
            info.Add(new InspectorValue("application-contentrootpath", "Content Root Path", environment.ContentRootPath));
            info.Add(new InspectorValue("application-webrootpath", "Web Root Path", environment.WebRootPath));
            info.Add(new InspectorValue("application-environmentname", "Environment Name", environment.EnvironmentName));
            return info;
        }

        private static IList<InspectorValue> GetSystemInfo()
        {
            var info = new List<InspectorValue>();
            info.Add(new InspectorValue("machine-name", "Machine Name", global::System.Environment.MachineName));
            info.Add(new InspectorValue("machine-uptime", "System Time", Format(DateTimeOffset.UtcNow)));
            info.Add(new InspectorValue("machine-processorcount", "Processor Count", global::System.Environment.ProcessorCount.ToString()));
            info.Add(new InspectorValue("os-is64bit", "64-bit OS", global::System.Environment.Is64BitOperatingSystem.ToString()));
            info.Add(new InspectorValue("os-version", "OS Version", global::System.Environment.OSVersion.ToString()));
            info.Add(new InspectorValue("process-is64bit", "64-bit Process", global::System.Environment.Is64BitProcess.ToString()));
            info.Add(new InspectorValue("process-uptime", "Process Uptime", GetProcessUptime().ToString()));
            info.Add(new InspectorValue("process-starttime", "Process Start Time", Format(DateTime.UtcNow - GetProcessUptime())));
            info.Add(new InspectorValue("clr-version", ".NET CLR Version", global::System.Environment.Version.ToString()));
            info.Add(new InspectorValue("clr-gcmode", ".NET Garbage Collection Mode", GCSettings.IsServerGC ? "Server" : "Workstation"));
            info.Add(new InspectorValue("loggedonuser-domain", "Logged On User Domain", global::System.Environment.UserDomainName));
            info.Add(new InspectorValue("loggedonuser-name", "Logged On User Name", global::System.Environment.UserName));
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