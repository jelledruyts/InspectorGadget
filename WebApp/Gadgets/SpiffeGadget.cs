using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using InspectorGadget.WebApp.Controllers;
using InspectorGadget.WebApp.Gadgets.Spiffe;
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Gadgets
{
    public class SpiffeGadget : GadgetBase<SpiffeGadget.Request, SpiffeGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string WorkloadApiAddress { get; set; }
            public string UnixDomainSocketEndpoint { get; set; }
            public string Audience { get; set; }

        }

        public class Result
        {
            public IList<SpiffeJwtSvid> JwtSvids { get; set; }
        }

        public SpiffeGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url, AppSettings appSettings)
            : base(logger, httpClientFactory, url.GetRelativeApiUrl(nameof(ApiController.Spiffe)), appSettings.DisableSpiffe)
        {
        }

        private async Task<Result> FetchJwtSvid(string workloadApiAddress, string unixDomainSocketEndpoint, string audience)
        {
            // Prepare Channel.
            var udsEndPoint = new UnixDomainSocketEndPoint(unixDomainSocketEndpoint); // default is "/tmp/spire-agent/public/api.sock"
            var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
            var socketsHttpHandler = new SocketsHttpHandler
            {
                ConnectCallback = connectionFactory.ConnectAsync
            };

            using var channel = GrpcChannel.ForAddress(workloadApiAddress, new GrpcChannelOptions
            {
                HttpHandler = socketsHttpHandler
            });

            // Prepare client.
            var client = new SpiffeWorkloadAPI.SpiffeWorkloadAPIClient(channel);

            // SPIFFE request.
            var request = new JWTSVIDRequest()
            {
                // SpiffeId = SpiffeId => only needed when a specific SPIFFE ID is requested
            };
            request.Audience.Add(audience);

            // SPIFFE metadata.
            var headers = new Metadata();
            headers.Add("workload.spiffe.io", "true");

            // Call SPIFFE workload endpoint.
            using var call = client.FetchJWTSVIDAsync(request, headers);
            var response = await call.ResponseAsync;
            var result = new Result();
            result.JwtSvids = response.Svids.Select(s => new SpiffeJwtSvid(s.SpiffeId, s.Svid, s.Hint)).ToArray();
            return result;
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            return await FetchJwtSvid(request.WorkloadApiAddress, request.UnixDomainSocketEndpoint, request.Audience);
        }

        private class UnixDomainSocketConnectionFactory
        {
            private readonly EndPoint endPoint;

            public UnixDomainSocketConnectionFactory(EndPoint endPoint)
            {
                this.endPoint = endPoint;
            }

            public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext context, CancellationToken cancellationToken = default)
            {
                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                try
                {
                    await socket.ConnectAsync(this.endPoint, cancellationToken).ConfigureAwait(false);
                    return new NetworkStream(socket, true);
                }
                catch
                {
                    socket.Dispose();
                    throw;
                }
            }
        }
    }
}