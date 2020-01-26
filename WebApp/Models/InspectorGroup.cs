using System.Collections.Generic;

namespace InspectorGadget.WebApp.Models
{
    public class InspectorGroup
    {
        public static IList<InspectorGroup> AllGroups { get; } = new List<InspectorGroup>
        {
            new InspectorGroup { Key = nameof(InspectorInfo.Request), DisplayName = "Request" },
            new InspectorGroup { Key = nameof(InspectorInfo.HttpHeaders), DisplayName = "HTTP Headers" },
            new InspectorGroup { Key = nameof(InspectorInfo.Identity), DisplayName = "Identity" },
            new InspectorGroup { Key = nameof(InspectorInfo.Configuration), DisplayName = "Configuration" },
            new InspectorGroup { Key = nameof(InspectorInfo.Environment), DisplayName = "Environment" },
            new InspectorGroup { Key = nameof(InspectorInfo.Application), DisplayName = "Application" },
            new InspectorGroup { Key = nameof(InspectorInfo.System), DisplayName = "System" },
        };

        public string Key { get; set; }
        public string DisplayName { get; set; }
    }
}