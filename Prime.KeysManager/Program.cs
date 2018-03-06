using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Prime.KeysManager.Transport;

namespace Prime.KeysManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new KeysManagerApp(new TcpServer());
            app.Run();   
        }
    }
}