using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.Core;
using Prime.SocketServer;

namespace Prime.ConsoleApp.Tests.Frank
{
    public class SocketServerTest
    {
        private ClientContext _cCtx;
        private ServerContext _sCtx;

        public void Go(ServerContext sCtx, ClientContext cCtx)
        {
            _sCtx = sCtx;
            _cCtx = cCtx;

            var mr = false;
            var server = new MessageServer(sCtx);

            sCtx.M.RegisterAsync<HelloRequest>(this, x =>
            {
                sCtx.M.Send(new HelloResponse(x));
            });

            cCtx.M.RegisterAsync<HelloResponse>(this, x =>
            {
                cCtx.Logger.Log(x.Response + " " + x.ClientId);
                mr = true;
            });

            server.Start();

            SendAsClient(server, cCtx.M, new HelloRequest());

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
            //dataString = "{\"type\":\"prime.keysmanager.privateproviderslistrequestmessage\"}";
            l.Log("Connection established, sending message: " + dataString);

            var dataBytes = dataString.GetBytes();

            client.Send(dataBytes);

            Task.Run(() =>
            {
                var helper = new MessageTypedSender(_cCtx.M);

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