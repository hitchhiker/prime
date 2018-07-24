using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using Prime.Base;
using Prime.Core;
using Prime.MessagingServer.Data;

namespace Prime.SocketServer.Transport
{
    public sealed class TypedServiceProvider : SocketServiceProvider
    {
        private readonly Server _server;
        private readonly CommonJsonDataProvider _dataProvider;

        public readonly ILogger L;
        public readonly IMessenger M;

        public TypedServiceProvider(Server server, IMessenger messenger = null)
        {
            _server = server;

            var msgServer = _server?.Context?.MessagingServer;

            L = msgServer?.L ?? new NullLogger();
            M = messenger ?? msgServer?.M;

            if(M == null)
                throw new ArgumentException("Messenger instance should be passed either as constructor argument or as property of Server.Context.MessegingServer.M.");

            _dataProvider = new CommonJsonDataProvider(_server.Context.MessagingServer);
        }

        public override object Clone()
        {
            return new TypedServiceProvider(_server, M);
        }

        public override void OnClientConnected(ObjectId clientId)
        {
            Log($"client '{clientId}' connected.");
        }

        public override void OnClientDisconnected(ObjectId clientId)
        {
            Log($"client '{clientId}' disconnected.");
        }

        public override void OnErrorOccurred(Exception e, ObjectId clientId)
        {
            Error($"error '{e.Message}', client id '{clientId}'.");
        }

        public override void OnDataReceived(ObjectId clientId, byte[] clonedData)
        {
            try
            {
                var response = clonedData.DecodeString();
                var m = UnpackResponse(response, clientId);

                if (m != null)
                    M.SendAsync(m);
            }
            catch (Exception e)
            {
                Error(e.Message);
            }
        }

        private ExternalMessage UnpackResponse(string response, ObjectId clientId)
        {
            return !(_dataProvider.Deserialize(response) is BaseTransportMessage message)
                ? null
                : new ExternalMessage(clientId, message);
        }

        public override void OnServerStarted(IPEndPoint ipEndPoint)
        {
            Log($"started on {ipEndPoint}.");
        }

        public override void OnServerStopped()
        {
            Log("stopped");
        }

        public override void Send<T>(ObjectId clientId, T data)
        {
            var tcpServer = TcpSocketServer;
            if(tcpServer == null)
                throw new NullReferenceException($"Unable send data through {nameof(TypedServiceProvider)} because it is null.");

            var dataJson = _dataProvider.Serialize(data).ToString();

            tcpServer.Send(clientId, dataJson.GetBytes());
        }

        #region Logging methods

        private void Log(string text, LoggingLevel loggingLevel = LoggingLevel.Status)
        {
            L.Log($"{SocketServerContext.LogServerName}: {text}", loggingLevel);
        }

        private void Error(string text)
        {
            Log(text, LoggingLevel.Error);
        }

        private void Warn(string text)
        {
            Log(text, LoggingLevel.Warning);
        }

        #endregion
    }
}
