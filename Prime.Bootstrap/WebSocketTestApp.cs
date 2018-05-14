using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.Core;
using Prime.Core.Testing;
using Prime.MessagingServer.Types;
using Prime.WebSocketServer;

namespace Prime.Bootstrap
{
    public class WebSocketTestApp
    {
        public WebSocketTestApp()
        {
        }
        
        public ServerContext S { get; set; }
        public ClientContext C { get; set; }

        public void Run() 
        {
            Console.WriteLine($"{nameof(WebSocketTestApp)} has started.");

            S = new ServerContext("..//..//..//instance//prime-server.config")
            {
                L = new ConsoleLogger()
            };

            C = new ClientContext("..//..//..//instance//prime-client.config")
            {
                L = new ConsoleLogger()
            };
            
            var prime = new Prime.Core.Prime(S);
            prime.Extensions.Loader.LoadAllBinDirectoryAssemblies();
            
            StartServer();
        }

        public void StartServer()
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
        }
    }
}
