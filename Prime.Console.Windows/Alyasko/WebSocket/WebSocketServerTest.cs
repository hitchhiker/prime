using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.Console.Windows.Frank.Socket;
using Prime.ConsoleApp.Tests.Frank;
using Prime.Core;
using Prime.Core.Testing;
using Prime.WebSocketServer;

namespace Prime.Console.Windows.Alyasko.WebSocket
{
    public class WebSocketServerTest : TestClientServerBase
    {
        public WebSocketServerTest(ServerContext server, ClientContext c) : base(server, c) { }

        public override void Go()
        {
            var mr = false;

            var server = new MessagingServer(S);
            server.Inject(new WsServerExtension());

            S.M.RegisterAsync<HelloRequest>(this, x =>
            {
                S.M.Send(new HelloResponse(x, "Hello World from WebSockets!"));
            });

            C.M.RegisterAsync<HelloResponse>(this, x =>
            {
                S.L.Log(x.Response + " " + x.SessionId);
                mr = true;
            });

            server.Start();

            SendAsClient(server, S.M, new HelloRequest());

            do
            {
                Thread.Sleep(1);
            } while (!mr);

            S.L.Log("Stopping WS server...");

            Thread.Sleep(1000);

            server.Stop();
        }

        public void SendAsClient(MessagingServer server, IMessenger msgr, BaseTransportMessage msg)
        {
            var ctx = new WsServerContext(server);
            var l = server.ServerContext.L;

            Task.Run(() =>
            {
                l.Log("Establishing connection to local socket server.");
                using (var ws = new WebSocketSharp.WebSocket($"ws://{IPAddress.Loopback}:{ctx.Port}/"))
                {
                    var settings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        SerializationBinder = server.TypeBinder
                    };

                    var helper = new MessageTypedSender(C.M);

                    ws.OnMessage += (sender, args) =>
                    {
                        if (args.IsText)
                        {
                            var dataText = args.Data;
                            if (string.IsNullOrWhiteSpace(dataText))
                                return;

                            if (JsonConvert.DeserializeObject(dataText, settings) is BaseTransportMessage m)
                                helper.UnPackSendReceivedMessage(new ExternalMessage(m.SessionId, m));
                        }
                        else
                        {
                            throw new InvalidOperationException("Only text data is supported.");
                        }
                    };

                    ws.Connect();

                    var json = JsonConvert.SerializeObject(new HelloRequest(), settings);

                    ws.Send(json);

                    while (ws.IsAlive) { }
                }
            });

            //var settings = new JsonSerializerSettings()
            //{
            //    TypeNameHandling = TypeNameHandling.Objects,
            //    SerializationBinder = server.TypeBinder
            //};

            //var dataString = JsonConvert.SerializeObject(msg, settings);

            //l.Log("Connection established, sending message: " + dataString);

            //var dataBytes = dataString.GetBytes();

            //client.Send(dataBytes);

            //Task.Run(() =>
            //{
            //    var helper = new MessageTypedSender(C.M);

            //    do
            //    {
            //        var buffer = new byte[1024];
            //        var iRx = client.Receive(buffer);
            //        var recv = buffer.GetString().Substring(0, iRx);

            //        if (string.IsNullOrWhiteSpace(recv))
            //            continue;

            //        if (JsonConvert.DeserializeObject(recv, settings) is BaseTransportMessage m)
            //            helper.UnPackSendReceivedMessage(new ExternalMessage(m.ClientId, m));

            //    } while (client.Connected);
            //});
        }
    }
}
