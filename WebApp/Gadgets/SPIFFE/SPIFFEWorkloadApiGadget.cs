using System;
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
using InspectorGadget.WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Gadgets
{
    public class SPIFFEWorkloadApiGadget : GadgetBase<SPIFFEWorkloadApiGadget.Request, SPIFFEWorkloadApiGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string UnixDomainSocketEndpoint { get; set; }
            public string Audience { get; set; }

        }

        public class Result
        {
            public IDictionary<string, string> JwtTokens { get; set; }
            public string Message { get; set; }

            public Result(){
                JwtTokens = new Dictionary<string, string>();
            }
        }

        public SPIFFEWorkloadApiGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url, AppSettings appSettings) : 
                                  base(logger, httpClientFactory, url.GetRelativeApiUrl("TODO"), appSettings.DisableSPIFFE){

        }

        private async Task<Result> FetchJwtSVID(string unixDomainSocketEndpoint, string audience){
            // Prepare Channel
            var udsEndPoint = new UnixDomainSocketEndPoint(unixDomainSocketEndpoint); // default is "/tmp/spire-agent/public/api.sock"
            var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
            var socketsHttpHandler = new SocketsHttpHandler
            {
                ConnectCallback = connectionFactory.ConnectAsync
            };

            using var channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            {
                HttpHandler = socketsHttpHandler
            });

            // Prepare client
            var client = new SPIFFE.SpiffeWorkloadAPI.SpiffeWorkloadAPIClient(channel);

            // SPIFFE request
            var req = new SPIFFE.JWTSVIDRequest(){
                // SpiffeId = SpiffeId => only needed when a specific SPIFFE ID is requested
            };
            req.Audience.Add(audience);
            
            // SPIFFE metadata
            Metadata headers = new();
            headers.Add("workload.spiffe.io","true");

            // Call SPIFFE workload endpoint
            Result result = new();
            try{
                using var call = client.FetchJWTSVIDAsync(req, headers);
                var response = await call.ResponseAsync;
                foreach(var svid in response.Svids){
                    result.JwtTokens.Add(svid.SpiffeId, svid.Svid);
                }
            }
            catch(RpcException exc){
                if(exc.Status.StatusCode == StatusCode.PermissionDenied && exc.Status.Detail == "no identity issued"){
                    // NO SVID was assigned
                    result.Message = "No SVID was assigned.  Grpc status code was PermissionDenied with detail: no identity issued";
                }
            }
            return result;
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            return await FetchJwtSVID(request.UnixDomainSocketEndpoint, request.Audience);
        }
    }

    public class UnixDomainSocketConnectionFactory
    {
        private readonly EndPoint _endPoint;

        public UnixDomainSocketConnectionFactory(EndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
            CancellationToken cancellationToken = default)
        {
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

            try
            {
                await socket.ConnectAsync(_endPoint, cancellationToken).ConfigureAwait(false);
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