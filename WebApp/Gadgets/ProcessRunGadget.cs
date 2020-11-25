using System;
using System.Net.Http;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Gadgets
{
    public class ProcessRunGadget : GadgetBase<ProcessRunGadget.Request, ProcessRunGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string FileName { get; set; }
            public string Arguments { get; set; }
            public int? TimeoutSeconds { get; set; }
        }

        public class Result
        {
            public int? ExitCode { get; set; }
            public string StandardOutput { get; set; }
            public string ErrorOutput { get; set; }
        }

        public ProcessRunGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url)
            : base(logger, httpClientFactory, url, nameof(ApiController.ProcessRun))
        {
        }

        protected override Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Running process \"{FileName}\" with arguments \"{Arguments}\"", request.FileName, request.Arguments);
            var timeout = request.TimeoutSeconds.HasValue ? (TimeSpan?)TimeSpan.FromSeconds(request.TimeoutSeconds.Value) : null;
            var result = ProcessRunner.RunProcess(request.FileName, request.Arguments, timeout, true, true);
            return Task.FromResult(new Result
            {
                ExitCode = result.ExitCode,
                StandardOutput = result.StandardOutput,
                ErrorOutput = result.StandardError
            });
        }
    }
}