using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;
using Prime.Core.Testing;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer.Transport
{
    public class WsCommonService : WebSocketBehavior
    {
        public IMessenger M;
        public ILogger L;
        public MessageServer MessageServer;

        public WebSocketSessionManager SessionManager;

        public WsCommonService()
        {
            SessionManager = Sessions;
        }

        public void SendData(string data)
        {
            Sessions.Broadcast(data);
            //base.Send(data);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            L.Log($"WsCommonService message received: '{e.Data}'.");

            var textData = e.Data;

            // Hack for now.
            var objId = ObjectId.NewObjectId();

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = MessageServer.TypeBinder
            };

            var m = !(JsonConvert.DeserializeObject(textData, settings) is BaseTransportMessage message)
                ? null
                : new ExternalMessage(objId, message);

            M.Send(m);
        }
    }
}
