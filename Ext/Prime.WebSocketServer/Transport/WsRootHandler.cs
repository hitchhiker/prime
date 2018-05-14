using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;
using Prime.Core.Testing;
using Prime.MessageServer.Data;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer.Transport
{
    public class WsRootHandler : WsHandlerBase
    {
        public const string ServicePath = "/";

        public JsonDataProvider DataProvider;

        protected override void OnMessage(MessageEventArgs e)
        {
            L.Log($"WsRootHandler message received: '{e.Data}'.");

            var textData = e.Data;

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = MessageServer.TypeBinder
            };

            var m = !(JsonConvert.DeserializeObject(textData, settings) is BaseTransportMessage message)
                ? null
                : new ExternalMessage(SessionId, message);

            M.Send(m);
        }
    }
}
