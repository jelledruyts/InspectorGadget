using System;
using System.Text.Json.Serialization;

namespace InspectorGadget.WebApp.Gadgets
{
    public class GadgetResponse
    {
        public string MachineName { get; set; } = Environment.MachineName;
        public DateTimeOffset TimeStarted { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset TimeCompleted { get; set; }
        public string Error { get; set; }
        [JsonIgnore]
        public object ResultObject { get; set; }
        [JsonIgnore]
        public GadgetResponse ChainedResponseObject { get; set; }
    }

    public class GadgetResponse<TResult> : GadgetResponse
    {
        public TResult Result
        {
            get { return (TResult)this.ResultObject; }
            set { this.ResultObject = value; }
        }

        public GadgetResponse<TResult> ChainedResponse
        {
            get { return (GadgetResponse<TResult>)this.ChainedResponseObject; }
            set { this.ChainedResponseObject = value; }
        }
    }
}