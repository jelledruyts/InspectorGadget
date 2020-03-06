using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using InspectorGadget.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;

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

        public SocketConnectionGadget(IHttpClientFactory httpClientFactory, IUrlHelper url)
            : base(httpClientFactory, url, nameof(ApiController.SocketConnection))
        {
        }

        protected override async Task<Result> ExecuteCoreAsync(Request request)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // Set a 20 second send and receive timeout.
                // Note that these are only respected by the synchronous Send and Receive methods, so we use
                // those instead of the async versions which don't appear to support timeouts or cancellation.
                socket.SendTimeout = 20000;
                socket.ReceiveTimeout = 20000;

                // Attempt to connect to the target host and port (this will time out after 20 seconds if it cannot connect).
                var result = new Result();
                await socket.ConnectAsync(request.RequestHostName, request.RequestPort);
                if (socket.Connected)
                {
                    result.Status = "Connected.";
                    if (!string.IsNullOrWhiteSpace(request.RequestBody))
                    {
                        socket.Send(new ArraySegment<byte>(Encoding.ASCII.GetBytes(request.RequestBody)), SocketFlags.None);
                        result.Status += " Request body sent.";
                    }
                    if (request.ReadResponse)
                    {
                        try
                        {
                            var responseBodyBuilder = new StringBuilder();
                            var bytesReceivedBuffer = new ArraySegment<byte>(new byte[256]);
                            var bytesReceived = 1;
                            while (bytesReceived > 0)
                            {
                                bytesReceived = socket.Receive(bytesReceivedBuffer, SocketFlags.None);
                                responseBodyBuilder.Append(Encoding.ASCII.GetString(bytesReceivedBuffer.Array, 0, bytesReceived));
                            }
                            result.ResponseBody = responseBodyBuilder.ToString();
                            result.Status += " Response read.";
                        }
                        catch (SocketException exc)
                        {
                            result.Status += $" Could not read response: {exc.Message}";
                        }
                    }
                }
                else
                {
                    result.Status += $"Could not connect to host \"{request.RequestHostName}\" on port {request.RequestPort}.";
                }
                return result;
            }
        }
    }
}