using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Prime.ConsoleApp.Tests.Frank;
using Prime.Core;
using Prime.WebSocketServer;
using Prime.WebSocketServer.Messages;

namespace Prime.Console.Windows.Alyasko.WebSocket
{
    public class WebSocketServerTest : TestClientServerBase
    {
        public WebSocketServerTest(ServerContext server, ClientContext c) : base(server, c) { }

        public override void Go()
        {
            var mr = false;

            var server = new MessageServer(S);
            server.Inject(new WsServerExtension());

            S.M.RegisterAsync<HelloWsRequest>(this, x =>
            {
                S.M.Send(new HelloWsResponse(x));
            });

            C.M.RegisterAsync<HelloWsResponse>(this, x =>
            {
                S.L.Log(x.Response + " " + x.ClientId);
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

        public void SendAsClient(MessageServer server, IMessenger msgr, BaseTransportMessage msg)
        {
            var ctx = new WsServerContext(server);
            var l = server.ServerContext.L;

            Task.Run(() =>
            {
                l.Log("Establishing connection to local socket server.");
                using (var ws = new WebSocketSharp.WebSocket($"ws://{IPAddress.Loopback}:{ctx.Port}/"))
                {
                    ws.OnMessage += (sender, args) =>
                    {
                        l.Log($"Received data: {args.Data}");
                    };

                    ws.Connect();
                    ws.Send("Hello from client!");

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
