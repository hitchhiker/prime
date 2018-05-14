using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.Core;
using Prime.Core.Testing;
using Prime.MessagingServer.Types;
using Prime.SocketServer;
using Server = Prime.MessagingServer.Server;
using ServerExtension = Prime.SocketServer.ServerExtension;

namespace Prime.Console.Frank.Socket
{
    public class SocketServerTest : TestClientServerBase
    {
        public SocketServerTest(ServerContext server, ClientContext c) : base(server, c) { }

        public override void Go()
        {
            var mr = false;

            var server = new Server(S);
            server.Inject(new ServerExtension());

            S.M.RegisterAsync<HelloRequest>(this, x =>
            {
                S.M.Send(new HelloResponse(x, "Hello World from Sockets!"));
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

            server.Stop();
        }

        public void SendAsClient(Server server, IMessenger msgr, BaseTransportMessage msg)
        {
            var ctx = new SocketServerContext(server);
            var l = server.ServerContext.L;

            var client = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
                        helper.UnPackSendReceivedMessage(new ExternalMessage(m.SessionId, m));

                } while (client.Connected);
            });
        }
    }
}