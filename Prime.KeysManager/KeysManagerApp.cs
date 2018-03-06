using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Prime.KeysManager.Events;
using Prime.KeysManager.Transport;

namespace Prime.KeysManager
{
    public class KeysManagerApp
    {
        private ITcpServer _tcpServer;
        
        public KeysManagerApp(ITcpServer tcpServer)
        {
            _tcpServer = tcpServer;
        }
        
        public void Run()
        {
            _tcpServer.ExceptionOccurred += TcpServerOnExceptionOccurred;
            _tcpServer.Subscribe<UserMessage>(message =>
            {
                Console.WriteLine($"User handled: {message.UserName}, {message.Age}");
            });
            _tcpServer.CreateServer(IPAddress.Parse("127.0.0.1"), 8082);     
        }

        private void TcpServerOnExceptionOccurred(object sender, Exception exception)
        {
            Console.WriteLine($"Error: {exception.Message}");
        }
    }
}