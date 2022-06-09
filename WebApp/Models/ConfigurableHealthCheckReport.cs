using System;

namespace InspectorGadget.WebApp.Models
{
    public class ConfigurableHealthCheckReport
    {
        public int Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public bool Healthy { get; set; }
        public string Description { get; set; }
    }
}