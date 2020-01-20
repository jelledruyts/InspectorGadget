using System;
using System.Collections.Generic;
using System.Linq;
using InspectorGadget.WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InspectorController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;

        public InspectorController(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.environment = environment;
            this.configuration = configuration;
        }

        [Route("{area?}/{key?}")]
        public object Get(string area, string key)
        {
            var info = InspectorInfo.Create(this.environment, this.configuration, this.Request);
            var values = default(IList<InspectorValue>);
            if (string.Equals(area, nameof(info.Application), StringComparison.InvariantCultureIgnoreCase))
            {
                values = info.Application;
            }
            else if (string.Equals(area, nameof(info.Configuration), StringComparison.InvariantCultureIgnoreCase))
            {
                values = info.Configuration;
            }
            else if (string.Equals(area, nameof(info.Environment), StringComparison.InvariantCultureIgnoreCase))
            {
                values = info.Environment;
            }
            else if (string.Equals(area, nameof(info.HttpHeaders), StringComparison.InvariantCultureIgnoreCase))
            {
                values = info.HttpHeaders;
            }
            else if (string.Equals(area, nameof(info.Identity), StringComparison.InvariantCultureIgnoreCase))
            {
                values = info.Identity;
            }
            else if (string.Equals(area, nameof(info.Request), StringComparison.InvariantCultureIgnoreCase))
            {
                values = info.Request;
            }
            else if (string.Equals(area, nameof(info.System), StringComparison.InvariantCultureIgnoreCase))
            {
                values = info.System;
            }

            if (values == null)
            {
                // No area was requested, or the area wasn't found, return the complete info object.
                return info;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    // No key was requested, return all values for the requested area.
                    return values;
                }
                else
                {
                    // A specific key was requested, return the first (if any) value.
                    return values.FirstOrDefault(v => v.Key == key)?.Value;
                }
            }
        }
    }
}