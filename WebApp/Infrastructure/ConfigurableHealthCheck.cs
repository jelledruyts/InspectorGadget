using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Infrastructure
{
    public class ConfigurableHealthCheck : IHealthCheck
    {
        public const string HealthCheckPath = "/health";
        private const int MaxHistoryItems = 20;
        private static object lockObject = new object();
        private static int nextConfigurableHealthCheckReportId = 1;
        public static ConfigurableHealthCheckMode Mode { get; private set; }
        public static int FailNextNumberOfTimes { get; private set; }
        public static IList<ConfigurableHealthCheckReport> History { get; private set; } = new List<ConfigurableHealthCheckReport>();

        public static void Configure(ConfigurableHealthCheckMode mode, int failNextNumberOfTimes)
        {
            lock (lockObject)
            {
                Mode = mode;
                FailNextNumberOfTimes = failNextNumberOfTimes;
            }
        }

        private readonly ILogger Logger;

        public ConfigurableHealthCheck(ILogger<ConfigurableHealthCheck> logger)
        {
            this.Logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            lock (lockObject)
            {
                var healthy = default(bool);
                var description = default(string);
                if (Mode == ConfigurableHealthCheckMode.AlwaysSucceed)
                {
                    healthy = true;
                    description = "Reporting healthy status because mode is set to always succeed";
                }
                else if (Mode == ConfigurableHealthCheckMode.AlwaysFail)
                {
                    healthy = false;
                    description = "Reporting unhealthy status because mode is set to always fail";
                }
                else if (Mode == ConfigurableHealthCheckMode.FailNextNumberOfTimes)
                {
                    if (FailNextNumberOfTimes > 0)
                    {
                        healthy = false;
                        description = $"Reporting unhealthy status because mode is set to fail for next {FailNextNumberOfTimes} times";
                        FailNextNumberOfTimes = Math.Max(0, FailNextNumberOfTimes - 1);
                    }
                    else
                    {
                        healthy = true;
                        description = $"Reporting healthy status because mode is set to fail for next {FailNextNumberOfTimes} times";
                    }
                }
                History.Add(new ConfigurableHealthCheckReport { Id = nextConfigurableHealthCheckReportId++, TimeStamp = DateTimeOffset.UtcNow, Healthy = healthy, Description = description });
                if (History.Count > MaxHistoryItems)
                {
                    History.RemoveAt(0);
                }
                this.Logger.LogInformation(description);
                if (healthy)
                {
                    return Task.FromResult(HealthCheckResult.Healthy(description));
                }
                else
                {
                    this.Logger.LogInformation(description);
                    return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, description));
                }
            }
        }
    }
}