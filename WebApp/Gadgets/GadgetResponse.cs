using System;

namespace InspectorGadget.WebApp.Gadgets
{
    public abstract class GadgetResponse<TRequest, TResponse> where TRequest : GadgetRequest<TRequest>
    {
        public TRequest Request { get; set; }
        public string MachineName { get; set; } = Environment.MachineName;
        public DateTimeOffset TimeStarted { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset TimeCompleted { get; set; }
        public string Error { get; set; }
        public TResponse ChainedResponse { get; set; }
    }
}