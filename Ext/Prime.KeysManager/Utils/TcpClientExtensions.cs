using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Prime.KeysManager.Utils
{
    public static class TcpClientExtensions
    {
        public static void Write(this TcpClient client, string data)
        {
            var bytes = Encoding.Default.GetBytes(data);
            client.GetStream().Write(bytes, 0, bytes.Length);
        }

        public static void WriteJson<T>(this TcpClient client, T instance)
        {
            var json = JsonConvert.SerializeObject(instance);

            Write(client, json);
        }
    }
}
