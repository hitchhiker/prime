using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Prime.KeysManager.Events;

namespace Prime.KeysManager.Transport
{
    public class TcpServer : ITcpServer
    {
        private TcpListener _listener;        
        private Dictionary<Type, Action<object>> _subscriptions;
        
        public TcpServer()
        {
            _subscriptions = new Dictionary<Type, Action<object>>();
        }
        
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
                if(!subscribedTypes.Any())
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
                var client = _listener.AcceptTcpClient();

                if (client != null)
                {
                    try
                    {
                        var message = "";

                        while (message != null && !message.StartsWith("quit"))
                        {
                            SendData(client, "> ");
                            ReceiveData(client, out var data);

                            HandleResponse(data);
                        }

                        client.GetStream().Dispose();
                    }
                    catch (Exception e)
                    {
                        ExceptionOccurred?.Invoke(this, e);
                    }
                }
            }
        }

        private bool SendData(TcpClient client, string data)
        {
            if (!client.Connected)
                return false;
            
            var dataBytes = Encoding.Default.GetBytes(data);  
            client.GetStream().Write(dataBytes, 0, data.Length);

            return true;
        }

        private bool ReceiveData(TcpClient client, out string data)
        {
            data = null;
            if (!client.Connected)
                return false;
            
            var buffer = new byte[1024];  
            client.GetStream().Read(buffer, 0, buffer.Length);  
   
            data = Encoding.Default.GetString(buffer);
            return true;
        }

        public void ShutdownServer()
        {
            _listener.Stop();
        }
        
        public void Subscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            if(_subscriptions.ContainsKey(t))
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

        public event EventHandler<Exception> ExceptionOccurred;
    }
}

