using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Prime.KeysManager.Utils;

namespace Prime.KeysManager.Transport
{
    public class JsonDataProvider : IDataProvider
    {
        public int ReceiveBufferSize = 1024;
        
        public bool SendData<T>(TcpClient client, T data)
        {
            if (!client.Connected)
                return false;

            var serializedData = Serialize(data);
            
            var dataString = serializedData.ToString();
            
            var dataBytes = Encoding.Default.GetBytes(dataString);
            client.GetStream().Write(dataBytes, 0, dataString.Length);

            return true;
        }

        public object ReceiveData(Stream stream)
        {
            var buffer = new byte[ReceiveBufferSize];
            if (stream.CanRead)
                stream.Read(buffer, 0, buffer.Length);

            return buffer.DecodeAscii();
        }

        public TType Deserialize<TType>(object serializedData)
        {
            return JsonConvert.DeserializeObject<TType>(serializedData.ToString());
        }

        public object Deserialize(object serializedData, Type targetType)
        {
            return JsonConvert.DeserializeObject(serializedData.ToString(), targetType);
        }

        public object Serialize<TType>(TType data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}