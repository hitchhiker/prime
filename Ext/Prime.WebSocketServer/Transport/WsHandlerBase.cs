using System;
using System.Collections.Concurrent;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using Prime.Base;
using Prime.Core;
using WebSocketSharp.Server;

namespace Prime.WebSocketServer.Transport
{
    public abstract class WsHandlerBase : WebSocketBehavior
    {
        public IMessenger M;
        public ILogger L;
        public MessagingServer.Server MessageServer;
        
        protected readonly ConcurrentDictionary<ObjectId, string> SessionsLookup = new ConcurrentDictionary<ObjectId, string>();

        /// <summary>
        /// Sends to client with specified ObjectId.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="objectId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SendTo(string data, ObjectId objectId)
        {
            if(!SessionsLookup.TryGetValue(objectId, out var remoteId)) 
                throw new InvalidOperationException("Client with specifiec objec");
            
            Sessions.SendTo(data, remoteId);
        }

        /// <summary>
        /// Sends data to currently connected client.
        /// </summary>
        /// <param name="data"></param>
        public void SendToCurrentClient(string data)
        {
            Send(data);
        }

        /// <summary>
        /// Broadcasts data to all connected clients.
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(string data)
        {
            Sessions.Broadcast(data);
        }

        protected string RemoteId => ID;

        private ObjectId _sessionId;
        protected ObjectId SessionId
        {
            get
            {
                if (_sessionId == null)
                {
                    _sessionId = RemoteId.GetObjectIdHashCode();
                    
                    if(!SessionsLookup.TryAdd(_sessionId, RemoteId))
                        throw new InvalidOperationException("Client with the same ObjectId has already been added.");
                }
                
                return _sessionId;
            }
        }
    }
}
