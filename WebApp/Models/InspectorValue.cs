namespace InspectorGadget.WebApp.Models
{
    public class InspectorValue
    {
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }

        public InspectorValue()
        {
        }

        public InspectorValue(string key, string displayName, string value)
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.Value = value;
        }
    }
}