using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Prime.Core
{
    public static class TcpClientExtensions
    {
        public static void WriteUtf32(this TcpClient client, string data)
        {
            var bytes = Encoding.UTF32.GetBytes(data);
            client.GetStream().Write(bytes, 0, bytes.Length);
        }

        public static void WriteJsonUtf32<T>(this TcpClient client, T instance)
        {
            var json = JsonConvert.SerializeObject(instance);

            WriteUtf32(client, json);
        }
    }
}