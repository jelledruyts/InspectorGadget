using System;
using InspectorGadget.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
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

        public static int? GetValueOrDefault(this IConfiguration value, string key, int? defaultValue)
        {
            var configValueString = value.GetValue<string>(key);
            if (!string.IsNullOrWhiteSpace(configValueString))
            {
                return int.TryParse(configValueString, out int configValue) ? configValue : defaultValue;
            }
            return defaultValue;
        }

        public static bool GetValueOrDefault(this IConfiguration value, string key, bool defaultValue)
        {
            return bool.TryParse(value.GetValue<string>(key), out bool configValue) ? configValue : defaultValue;
        }

        public static TEnum GetValueOrDefault<TEnum>(this IConfiguration value, string key, TEnum defaultValue) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value.GetValue<string>(key), out TEnum configValue) ? configValue : defaultValue;
        }

        public static string GetRelativeApiUrl(this IUrlHelper url, string apiActionName)
        {
            return url.Action(apiActionName, ApiController.ControllerName);
        }
    }
}