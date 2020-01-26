using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InspectorGadget.WebApp.Gadgets
{
    public class IntrospectorGadget
    {
        public class Request : GadgetRequest<Request>
        {
            public string Group { get; set; }
            public string Key { get; set; }

            public override Request Clone()
            {
                return new Request { Group = this.Group, Key = this.Key };
            }
        }

        public class Response : GadgetResponse<Request, Response>
        {
            public string ResponseBody { get; set; }
        }

        public static async Task<Response> ExecuteAsync(Request request, string relativeUrl, HttpRequest httpRequest, IWebHostEnvironment environment, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            var response = new Response { Request = request };
            try
            {
                var info = InspectorInfo.Create(environment, configuration, httpRequest, false);
                var value = info.GetPart(request.Group, request.Key);
                if (value is string)
                {
                    response.ResponseBody = value.ToString();
                }
                else
                {
                    response.ResponseBody = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception exc)
            {
                response.Error = exc.ToString();
            }
            response.ChainedResponse = await request.PerformCallChainAsync<Response>(httpClientFactory, relativeUrl);
            response.TimeCompleted = DateTimeOffset.UtcNow;
            return response;
        }
    }
}