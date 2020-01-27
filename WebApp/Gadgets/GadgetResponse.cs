using System;

namespace InspectorGadget.WebApp.Gadgets
{
    public class GadgetResponse<TResult>
    {
        public string MachineName { get; set; } = Environment.MachineName;
        public DateTimeOffset TimeStarted { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset TimeCompleted { get; set; }
        public string Error { get; set; }
        public TResult Result { get; set; }
        public GadgetResponse<TResult> ChainedResponse { get; set; }
    }
}