using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InspectorGadget.WebApp.Gadgets
{
    public class SocketConnectionGadget : GadgetBase<SocketConnectionGadget.Request, SocketConnectionGadget.Result>
    {
        public class Request : GadgetRequest
        {
            public string RequestHostName { get; set; }
            public int RequestPort { get; set; }
            public string RequestBody { get; set; }
            public bool ReadResponse { get; set; }
        }

        public class Result
        {
            public string Status { get; set; }
            public string ResponseBody { get; set; }
        }

        public SocketConnectionGadget(ILogger logger, IHttpClientFactory httpClientFactory, IUrlHelper url)
            : base(logger, httpClientFactory, url, nameof(ApiController.SocketConnection))
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            this.Logger.LogInformation("Establishing Socket Connection for RequestHostName {RequestHostName} and RequestPort {RequestPort}", request.RequestHostName, request.RequestPort);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // Set a 20 second send and receive timeout.
                // Note that these are only respected by the synchronous Send and Receive methods, so we use
                // those instead of the async versions which don't appear to support timeouts or cancellation.
                socket.SendTimeout = 20000;
                socket.ReceiveTimeout = 20000;

                // Attempt to connect to the target host and port (this will time out after 20 seconds if it cannot connect).
                var result = new Result();
                this.Logger.LogDebug("Connecting to socket");
                await socket.ConnectAsync(request.RequestHostName, request.RequestPort);
                if (socket.Connected)
                {
                    this.Logger.LogDebug("Connected to socket");
                    result.Status = "Connected.";
                    if (!string.IsNullOrWhiteSpace(request.RequestBody))
                    {
                        this.Logger.LogDebug("Sending request bytes over socket");
                        socket.Send(new ArraySegment<byte>(Encoding.ASCII.GetBytes(request.RequestBody)), SocketFlags.None);
                        result.Status += " Request body sent.";
                    }
                    if (request.ReadResponse)
                    {
                        this.Logger.LogDebug("Reading response from socket");
                        try
                        {
                            var responseBodyBuilder = new StringBuilder();
                            var bytesReceivedBuffer = new ArraySegment<byte>(new byte[256]);
                            var bytesReceived = 1;
                            while (bytesReceived > 0)
                            {
                                this.Logger.LogDebug("Read response buffer from socket");
                                bytesReceived = socket.Receive(bytesReceivedBuffer, SocketFlags.None);
                                responseBodyBuilder.Append(Encoding.ASCII.GetString(bytesReceivedBuffer.Array, 0, bytesReceived));
                            }
                            result.ResponseBody = responseBodyBuilder.ToString();
                            result.Status += " Response read.";
                            this.Logger.LogDebug("Read response from socket");
                        }
                        catch (SocketException exc)
                        {
                            this.Logger.LogError(exc, "Could not read response from socket");
                            result.Status += $" Could not read response: {exc.Message}";
                        }
                    }
                }
                else
                {
                    this.Logger.LogError("Could not connect socket");
                    result.Status += $"Could not connect to host \"{request.RequestHostName}\" on port {request.RequestPort}.";
                }
                return result;
            }
        }
    }
}