using System;

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
    }
}