using InspectorGadget.WebApp.Gadgets;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp
{
    public class AppSettings
    {
        private readonly IConfiguration configuration;
        
        public string BackgroundColor => configuration.GetValueOrDefault("BackgroundColor", default(string));
        public string InfoMessage => configuration.GetValueOrDefault("InfoMessage", default(string));
        
        public string DefaultCallChainUrls => configuration.GetValueOrDefault("DefaultCallChainUrls", default(string));
        
        public bool DisableAzureManagedIdentity => configuration.GetValueOrDefault("DisableAzureManagedIdentity", false);
        public string DefaultAzureManagedIdentityScopes => configuration.GetValueOrDefault("DefaultAzureManagedIdentityScopes", "https://management.azure.com/.default");
        public string DefaultAzureManagedIdentityClientId => configuration.GetValueOrDefault("DefaultAzureManagedIdentityClientId", default(string));
        
        public bool DisableDnsLookup => configuration.GetValueOrDefault("DisableDnsLookup", false);
        public string DefaultDnsLookupHost => configuration.GetValueOrDefault("DefaultDnsLookupHost", default(string));
        
        public bool DisableHttpRequest => configuration.GetValueOrDefault("DisableHttpRequest", false);
        public string DefaultHttpRequestUrl => configuration.GetValueOrDefault("DefaultHttpRequestUrl", "http://ipinfo.io/ip");
        public string DefaultHttpRequestHostName => configuration.GetValueOrDefault("DefaultHttpRequestHostName", default(string));
        
        public bool DisableIntrospector => configuration.GetValueOrDefault("DisableIntrospector", false);
        public string DefaultIntrospectorGroup => configuration.GetValueOrDefault("DefaultIntrospectorGroup", default(string));
        public string DefaultIntrospectorKey => configuration.GetValueOrDefault("DefaultIntrospectorKey", default(string));
        
        public bool DisableProcessRun => configuration.GetValueOrDefault("DisableProcessRun", false);
        public string DefaultProcessRunFileName => configuration.GetValueOrDefault("DefaultProcessRunFileName", default(string));
        public string DefaultProcessRunArguments => configuration.GetValueOrDefault("DefaultProcessRunArguments", default(string));
        public int? DefaultProcessRunTimeoutSeconds => configuration.GetValueOrDefault("DefaultProcessRunTimeoutSeconds", default(int?));
        
        public bool DisableSocketConnection => configuration.GetValueOrDefault("DisableSocketConnection", false);
        public string DefaultSocketConnectionRequestHostName => configuration.GetValueOrDefault("DefaultSocketConnectionRequestHostName", "ipinfo.io");
        public int DefaultSocketConnectionRequestPort => configuration.GetValueOrDefault("DefaultSocketConnectionRequestPort", 80);
        public string DefaultSocketConnectionRequestBody => configuration.GetValueOrDefault("DefaultSocketConnectionRequestBody", $"GET / HTTP/1.1\r\nHost: {DefaultSocketConnectionRequestHostName}\r\nConnection: Close\r\n\r\n");
        public bool DefaultSocketConnectionReadResponse => configuration.GetValueOrDefault("DefaultSocketConnectionReadResponse", true);
        
        public bool DisableSqlConnection => configuration.GetValueOrDefault("DisableSqlConnection", false);
        public string DefaultSqlConnectionDatabaseType => configuration.GetValueOrDefault("DefaultSqlConnectionDatabaseType", default(string));
        public string DefaultSqlConnectionSqlConnectionString => configuration.GetValueOrDefault("DefaultSqlConnectionSqlConnectionString", default(string));
        public string DefaultSqlConnectionSqlQuery => configuration.GetValueOrDefault("DefaultSqlConnectionSqlQuery", SqlConnectionGadget.SqlServerDefaultQuery);
        public bool DefaultSqlConnectionUseAzureManagedIdentity => configuration.GetValueOrDefault("DefaultSqlConnectionUseAzureManagedIdentity", false);
        public string DefaultSqlConnectionAzureManagedIdentityClientId => configuration.GetValueOrDefault("DefaultSqlConnectionAzureManagedIdentityClientId", default(string));

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}