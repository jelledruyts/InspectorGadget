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
            public IList<SpiffeX509Svid> X509Svids { get; set; }
        }

        public SpiffeGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url, AppSettings appSettings)
            : base(logger, httpClientFactory, url.GetRelativeApiUrl(nameof(ApiController.Spiffe)), appSettings.DisableSpiffe)
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            // Prepare Channel.
            var udsEndPoint = new UnixDomainSocketEndPoint(request.UnixDomainSocketEndpoint); // default is "/tmp/spire-agent/public/api.sock"
            var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
            var socketsHttpHandler = new SocketsHttpHandler
            {
                ConnectCallback = connectionFactory.ConnectAsync
            };

            using var channel = GrpcChannel.ForAddress(request.WorkloadApiAddress, new GrpcChannelOptions
            {
                HttpHandler = socketsHttpHandler
            });

            // Prepare client.
            var client = new SpiffeWorkloadAPI.SpiffeWorkloadAPIClient(channel);

            // SPIFFE metadata.
            var headers = new Metadata();
            headers.Add("workload.spiffe.io", "true");

            var result = new Result();

            // Call SPIFFE workload endpoint for JWT-SVID request.
            var jwtSvidRequest = new JWTSVIDRequest
            {
                // SpiffeId = SpiffeId => only needed when a specific SPIFFE ID is requested
            };
            jwtSvidRequest.Audience.Add(request.Audience);
            using var jwtCall = client.FetchJWTSVIDAsync(jwtSvidRequest, headers);
            var jwtResponse = await jwtCall.ResponseAsync;
            result.JwtSvids = jwtResponse.Svids.Select(s => new SpiffeJwtSvid(s.SpiffeId, s.Svid, s.Hint)).ToArray();

            // Call SPIFFE workload endpoint for X509-SVID request.
            var x509SvidRequest = new X509SVIDRequest();
            using var x509Call = client.FetchX509SVID(x509SvidRequest, headers);
            if (await x509Call.ResponseStream.MoveNext())
            {
                result.X509Svids = x509Call.ResponseStream.Current.Svids.Select(s => new SpiffeX509Svid(s)).ToArray();
            }

            return result;
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