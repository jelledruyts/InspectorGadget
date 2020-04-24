using System;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Infrastructure
{
    public static class ExtensionMethods
    {
        private const string DateTimeFormatString = "yyyy-MM-dd hh:mm:ss.fffffff (UTC)";

        public static string ToDisplayString(this DateTimeOffset value)
        {
            return value.ToString(DateTimeFormatString);
        }

        public static string ToDisplayString(this DateTime value)
        {
            return value.ToString(DateTimeFormatString);
        }

        public static string GetValueOrDefault(this IConfiguration value, string key, string defaultValue)
        {
            var configValue = value.GetValue<string>(key);
            return string.IsNullOrWhiteSpace(configValue) ? defaultValue : configValue;
        }

        public static int GetValueOrDefault(this IConfiguration value, string key, int defaultValue)
        {
            return int.TryParse(value.GetValue<string>(key), out int configValue) ? configValue : defaultValue;
        }

        public static bool GetValueOrDefault(this IConfiguration value, string key, bool defaultValue)
        {
            return bool.TryParse(value.GetValue<string>(key), out bool configValue) ? configValue : defaultValue;
        }
    }
}