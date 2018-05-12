using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.ConsoleApp.Tests.Frank;
using Prime.Core;
using Prime.SocketServer;
using Prime.WebSocketServer;

namespace Prime.Console.Alyasko.WebSocket
{
    public class WebSocketServerTest : TestClientServerBase
    {
        public WebSocketServerTest(ServerContext server, ClientContext c) : base(server, c) { }

        public override void Go()
        {
            var mr = false;

            var server = new MessageServer(S);
            server.Inject(new WebSocketServerExtension());

            S.M.RegisterAsync<HelloRequest>(this, x =>
            {
                S.M.Send(new HelloResponse(x));
            });

            C.M.RegisterAsync<HelloResponse>(this, x =>
            {
                S.L.Log(x.Response + " " + x.ClientId);
                mr = true;
            });

            server.Start();

            // SendAsClient(server, S.M, new HelloRequest());

            do
            {
                Thread.Sleep(1);
            } while (!mr);

            server.Stop();
        }

        public void SendAsClient(MessageServer server, IMessenger msgr, BaseTransportMessage msg)
        {
            var ctx = new SocketServerContext(server);
            var l = server.ServerContext.L;

            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            l.Log("Establishing connection to local socket server.");
            client.Connect("127.0.0.1", ctx.PortNumber);

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = server.TypeBinder
            };

            var dataString = JsonConvert.SerializeObject(msg, settings);

            l.Log("Connection established, sending message: " + dataString);

            var dataBytes = dataString.GetBytes();

            client.Send(dataBytes);

            Task.Run(() =>
            {
                var helper = new MessageTypedSender(C.M);

                do
                {
                    var buffer = new byte[1024];
                    var iRx = client.Receive(buffer);
                    var recv = buffer.GetString().Substring(0, iRx);

                    if (string.IsNullOrWhiteSpace(recv))
                        continue;

                    if (JsonConvert.DeserializeObject(recv, settings) is BaseTransportMessage m)
                        helper.UnPackSendReceivedMessage(new ExternalMessage(m.ClientId, m));

                } while (client.Connected);
            });
        }
    }
}
