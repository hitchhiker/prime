using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Prime.Base;
using Prime.Core;
using Prime.Core.Testing;
using Prime.MessagingServer.Data;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer.Transport
{
    public class WsRootHandler : WsHandlerBase
    {
        public const string ServicePath = "/";

        public CommonJsonDataProvider DataProvider;

        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = !(DataProvider.Deserialize(e.Data) is BaseTransportMessage baseMsg)
                ? null
                : new ExternalMessage(SessionId, baseMsg);
            
            M.Send(msg);
        }
    }
}
