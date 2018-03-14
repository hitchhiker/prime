using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Prime.KeysManager.Messages;
using Prime.KeysManager.Utils;

namespace Prime.KeysManager.Transport
{
    public class TcpServer : ITcpServer
    {
        private TcpListener _listener;
        private TcpClient _connectedClient;
        private readonly Dictionary<Type, Action<object>> _subscriptions = new Dictionary<Type, Action<object>>();

        public void CreateServer(IPAddress address, short port)
        {
            _listener = new TcpListener(address, port);
            _listener.Start();

            AcceptClient();
        }

        private void HandleResponse(string raw)
        {
            try
            {
                var baseMessage = JsonConvert.DeserializeObject<BaseMessage>(raw);

                var subscribedTypes = _subscriptions.Where(x =>
                    x.Key.Name.Equals(baseMessage.Type, StringComparison.OrdinalIgnoreCase)).ToList();
                if (!subscribedTypes.Any())
                    throw new NullReferenceException($"There is no handler registered for '{baseMessage.Type}' type.");

                var subscribedType = subscribedTypes.First();
                if (subscribedType.Value != null)
                {
                    var parameter = JsonConvert.DeserializeObject(raw, subscribedType.Key);
                    subscribedType.Value(parameter);
                }
                else
                {
                    throw new NullReferenceException($"Subscribed handler for '{baseMessage.Type}' type is null.");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void AcceptClient()
        {
            while (true)
            {
                using (_connectedClient = _listener.AcceptTcpClient())
                {
                    try
                    {
                        using (var stream = _connectedClient.GetStream())
                        {
                            while (_connectedClient.Connected)
                            {
                                ReceiveData(stream, out var data);

                                data = CleanData(data);

                                if (!string.IsNullOrWhiteSpace(data))
                                    HandleResponse(data);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionOccurred?.Invoke(this, e);
                    }
                }
            }
        }

        private string CleanData(string data)
        {
            return data.Replace("\r", "").Replace("\n", "");
        }

        private bool SendData(TcpClient client, string data)
        {
            if (!client.Connected)
                return false;

            var dataBytes = Encoding.Default.GetBytes(data);
            client.GetStream().Write(dataBytes, 0, data.Length);

            return true;
        }

        private void ReceiveData(NetworkStream stream, out string data)
        {
            data = null;

            var buffer = new byte[1024];
            if(stream.CanRead)
                stream.Read(buffer, 0, buffer.Length);

            data = buffer.DecodeAscii();//Encoding.Default.GetString(buffer);
        }

        public void ShutdownServer()
        {
            _listener.Stop();
            _connectedClient.Dispose();
            Console.WriteLine("Server closed");
        }

        public void Subscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            if (_subscriptions.ContainsKey(t))
                throw new InvalidOperationException("Handler for specified type has already been added.");

            _subscriptions.Add(t, (o) => handler((T)o));
        }

        public void Unsubscribe<T>()
        {
            var t = typeof(T);
            if (_subscriptions.ContainsKey(t))
                _subscriptions.Remove(t);
            else
                throw new InvalidOperationException("Handler for specified type does not exist.");
        }

        public void Send<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data);
            SendData(_connectedClient, json);
        }

        public event EventHandler<Exception> ExceptionOccurred;
    }
}

