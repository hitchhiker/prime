using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.WebSocketServer.Messages;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer.Transport
{
    public class WsCommonService : WebSocketBehavior
    {
        public IMessenger M;
        public ILogger L;

        public WebSocketSessionManager SessionManager;

        public WsCommonService()
        {
            SessionManager = Sessions;
        }

        public void SendData(string data)
        {
            base.Send(data);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            L.Log($"WsCommonService message received: '{e.Data}'.");
        }
    }
}
